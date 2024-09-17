using Foundation.Exceptions;
using Microsoft.Extensions.Logging;
using RedLockNet.SERedis;
using RedLockNet.SERedis.Configuration;
using StackExchange.Redis;
using System.Text.Json;

namespace RedisCachingService
{
    public class RedisCacheRepository : IRedisCacheRepository
    {
        private readonly IDatabase db;
        private readonly TimeSpan expiryTime;
        private readonly ILogger<RedisCacheRepository> logger;
        private readonly RedLockFactory redLockFactory;

        public RedisCacheRepository(ConnectionMultiplexer connectionMultiplexer, 
            TimeSpan expiryTime, ILogger<RedisCacheRepository> logger)
        {
            this.logger = logger;
            this.expiryTime = expiryTime;
            db = connectionMultiplexer.GetDatabase();
            redLockFactory = RedLockFactory.Create(new List<RedLockMultiplexer> { connectionMultiplexer });
        }

        public async Task<bool> AddEntityToHashAsync(string key, IDictionary<string, string> dictionary, CancellationToken cancellationToken)
        {
            int threadId = Thread.CurrentThread.ManagedThreadId;
            var hashEntries = dictionary.Select(kvp => new HashEntry(kvp.Key, kvp.Value)).ToArray();

            return await AcquireLockAsync(key, async (ct) =>
            {
                var existingEntries = await db.HashGetAllAsync(key);
                if (existingEntries.Any())
                {
                    logger.LogInformation($"Entity already exists for key {key}. Skipping update. ThreadId {threadId}");
                    return false;
                }

                await db.HashSetAsync(key, hashEntries);
                logger.LogInformation($"Entity {JsonSerializer.Serialize(dictionary)} add for key {key}. ThreadId {threadId}");
                await db.KeyExpireAsync(key, expiryTime);
                return true;
            }, cancellationToken);
        }

        public async Task<IDictionary<string, string>> GetEntityFromHashAsync(string hashKey)
        {
            try
            {
                var values = await db.HashGetAllAsync(hashKey);
                return values.ToDictionary(e => (string)e.Name!, e => (string)e.Value!);
            }
            catch (RedisException ex)
            {
                logger.LogError($"Error getting all hash values for key {hashKey}");
                throw new CustomRedisException($"Error getting all hash values for key {hashKey}", ex.Message);
            }
        }

        public async Task<bool> AddEntryToHashAsync(string key, string entryName, string entryValue, CancellationToken cancellationToken)
        {
            return await AcquireLockAsync(key, async (ct) =>
            {
                int threadId = Thread.CurrentThread.ManagedThreadId;
                var existingValue = await db.HashGetAsync(key, entryName);
                if (existingValue.HasValue)
                {
                    logger.LogError($"Entry already exists for key {key} and id {entryName}. Skipping update. ThreadId {threadId}");
                    return false;
                }

                await db.HashSetAsync(key, [new HashEntry(entryName, entryValue)]);
                logger.LogInformation($"Entry {JsonSerializer.Serialize(new HashEntry(entryName, entryValue))} add for key {key}. ThreadId {threadId}");
                await db.KeyExpireAsync(key, expiryTime);
                return true;
            }, cancellationToken);
        }

        public async Task<string> GetEntryValueFromHashAsync(string key, string entryName)
        {
            try
            {
                var value = await db.HashGetAsync(key, entryName);
                return value.ToString();
            }
            catch (RedisException ex)
            {
                logger.LogError($"Error getting hash data for key {key} and field {entryName}");
                throw new CustomRedisException($"Error getting hash data for key {key} and field {entryName}", ex.Message);
            }
        }

        public async Task<bool> AddValueToSetAsync(string key, string value, CancellationToken cancellationToken)
        {
            int threadId = Thread.CurrentThread.ManagedThreadId;
            return await AcquireLockAsync(key, async (ct) =>
            {
                var members = await db.SetMembersAsync(key);
                if (members.Contains(value))
                {
                    logger.LogInformation($"Value {value} already exists in index {key} in set. Skipping update. ThreadId {threadId}");
                    return false;
                }

                await db.SetAddAsync(key, value);
                logger.LogInformation($"Value {value} added to set to index {key}. ThreadId {threadId}");
                
                return true;
            }, cancellationToken);
        }

        public async Task<List<string>> GetValuesFromSetAsync(string key)
        {
            try
            {
                var redisValues = await db.SetMembersAsync(key);
                return redisValues.Select(rv => (string)rv!).ToList();
            }
            catch (RedisException ex)
            {
                logger.LogError($"Error getting members of set with {key}");
                throw new CustomRedisException($"Error getting members of set with {key}", ex.Message);
            }
        }

        public async Task<bool> AddStringAsync(string key, string value, CancellationToken cancellationToken)
        {
            int threadId = Thread.CurrentThread.ManagedThreadId;
            return await AcquireLockAsync(key, async (ct) =>
            {
                logger.LogInformation($"Save string {value} with {key}. ThreadId {threadId}");
                await db.StringSetAsync(key, value, expiryTime);
                return true;
            }, cancellationToken);
        }

        public async Task<string> GetStringAsync(string key)
        {
            try
            {
                var result = await db.StringGetAsync(key);

                if (result.IsNullOrEmpty)
                {
                    logger.LogInformation($"No cached string found for key {key}");
                    return string.Empty;
                }

                return result.ToString();
            }
            catch (RedisException ex)
            {
                logger.LogError($"Error getting value for key {key}");
                throw new CustomRedisException($"Redis error for key {key}", ex.Message);
            }
        }

        public Task<bool> DeleteEntryFromHashAsync(string key, string entryName, CancellationToken cancellationToken)
        {
            return AcquireLockAsync(key, async (ct) => await db.HashDeleteAsync(key, entryName), cancellationToken);
        }

        public Task<bool> DeleteEntityFromHashAsync(string key, CancellationToken cancellationToken = default)
        {
            return AcquireLockAsync(key, async (ct) => await db.KeyDeleteAsync(key), cancellationToken);
        }

        public async Task<bool> AcquireLockAsync(
            string key, 
            Func<CancellationToken, Task<bool>> action, 
            CancellationToken cancellationToken = default, 
            int maxRetries = 5)
        {
            int retries = 0;
            var waitTime = TimeSpan.FromSeconds(10);  // Maximum time to wait for the lock
            var retryTime = TimeSpan.FromMilliseconds(200);  // Interval between retries

            int threadId = Thread.CurrentThread.ManagedThreadId;

            while (retries < maxRetries)
            {
                retries++;
                try
                {
                    await using (var redLock = await redLockFactory.CreateLockAsync(key, expiryTime, waitTime, retryTime, cancellationToken))
                    {
                        if (redLock.IsAcquired)
                        {
                            var result = await action(cancellationToken);
                            logger.LogInformation($"ThreadId {threadId} made action.");
                            return result;
                        }
                        // If the lock was not acquired, wait for retryTime and try again.
                    }
                }
                catch (Exception ex)
                {
                    logger.LogError($"Redis error performing action with key {key}: {ex.Message}");
                    return false;
                }
            }

            logger.LogError($"Unable to acquire lock for key {key}.");
            return false;
        }
    }
}