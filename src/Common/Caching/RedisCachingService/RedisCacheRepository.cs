using Foundation.Exceptions;
using RedLockNet.SERedis;
using RedLockNet.SERedis.Configuration;
using StackExchange.Redis;

namespace RedisCachingService
{
    public class RedisCacheRepository : IRedisCacheRepository
    {
        private readonly IDatabase db;
        private readonly TimeSpan expiryTime;
        private readonly RedLockFactory redLockFactory;

        public RedisCacheRepository(ConnectionMultiplexer connectionMultiplexer, TimeSpan expiryTime)
        {
            this.expiryTime = expiryTime;
            db = connectionMultiplexer.GetDatabase();
            redLockFactory = RedLockFactory.Create(new List<RedLockMultiplexer> { connectionMultiplexer });
        }

        public async Task<bool> AddEntityToHashAsync(string key, IDictionary<string, string> dictionary)
        {
            var hashEntries = dictionary.Select(kvp => new HashEntry(kvp.Key, kvp.Value)).ToArray();

            return await AcquireLockAsync(key, async () =>
            {
                var existingEntries = await db.HashGetAllAsync(key);
                if (existingEntries.Any())
                {
                    return false;
                }

                await db.HashSetAsync(key, hashEntries);
                await db.KeyExpireAsync(key, expiryTime);
                return true;
            });
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
                throw new CustomRedisException($"Error getting all hash values for key {hashKey}", ex.Message);
            }
        }

        public async Task<bool> AddEntryToHashAsync(string key, string entryName, string entryValue)
        {
            return await AcquireLockAsync(key, async () =>
            {
                var existingValue = await db.HashGetAsync(key, entryName);
                if (existingValue.HasValue)
                {
                    return false;
                }

                await db.HashSetAsync(key, [new HashEntry(entryName, entryValue)]);
                await db.KeyExpireAsync(key, expiryTime);
                return true;
            });
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
                throw new CustomRedisException($"Error getting hash data for key {key} and field {entryName}", ex.Message);
            }
        }

        public async Task<bool> AddValueToSetAsync(string key, string value)
        {
            return await AcquireLockAsync(key, async () =>
            {
                var members = await db.SetMembersAsync(key);
                if (members.Contains(value))
                {
                    return false;
                }

                await db.SetAddAsync(key, value);
                return true;
            });
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
                throw new CustomRedisException($"Error getting members of set with {key}", ex.Message);
            }
        }

        public async Task<bool> AddStringAsync(string key, string value)
        {
            return await AcquireLockAsync(key, async () =>
            {
                await db.StringSetAsync(key, value, expiryTime);
                return true;
            });
        }

        public async Task<string> GetStringAsync(string key)
        {
            try
            {
                var result = await db.StringGetAsync(key);

                if (result.IsNullOrEmpty)
                {
                    return string.Empty;
                }

                return result.ToString();
            }
            catch (RedisException ex)
            {
                throw new CustomRedisException($"Redis error for key {key}", ex.Message);
            }
        }

        public Task<bool> DeleteEntryFromHashAsync(string key, string entryName)
        {
            return AcquireLockAsync(key, async () => await db.HashDeleteAsync(key, entryName));
        }

        public Task<bool> DeleteEntityFromHashAsync(string key)
        {
            return AcquireLockAsync(key, async () => await db.KeyDeleteAsync(key));
        }

        public async Task<bool> AcquireLockAsync(
            string key, 
            Func<Task<bool>> action,  
            int maxRetries = 5)
        {
            int retries = 0;
            var waitTime = TimeSpan.FromSeconds(10);  // Maximum time to wait for the lock
            var retryTime = TimeSpan.FromMilliseconds(200);  // Interval between retries

            while (retries < maxRetries)
            {
                retries++;
                try
                {
                    await using (var redLock = await redLockFactory.CreateLockAsync(key, expiryTime, waitTime, retryTime))
                    {
                        if (redLock.IsAcquired)
                        {
                            var result = await action();
                            return result;
                        }
                        // If the lock was not acquired, wait for retryTime and try again.
                    }
                }
                catch (Exception ex)
                {
                    throw new CustomRedisException($"Redis error performing action with key {key}", ex.Message);
                }
            }

            throw new CustomRedisException($"Unable to acquire lock for key {key}");
        }
    }
}