using Microsoft.Extensions.Configuration;
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

        public RedisCacheRepository(IConfiguration configuration)
        {
            string connectionString = configuration.GetValue<string>("CacheSettings:RedisConnectionString")!;
            expiryTime = TimeSpan.FromMinutes(configuration.GetValue<double>("CacheSettings:DefaultCacheDurationMinutes"));
            var connectionMultiplexer = ConnectionMultiplexer.Connect(connectionString);
            db = connectionMultiplexer.GetDatabase();
            redLockFactory = RedLockFactory.Create(new List<RedLockMultiplexer> { connectionMultiplexer });
        }

        public async Task AddEntityToHashAsync(string key, IDictionary<string, string> dictionary, CancellationToken cancellationToken)
        {
            var hashEntries = dictionary.Select(kvp => new HashEntry(kvp.Key, kvp.Value)).ToArray();

            await AcquireLockAsync(key, async (ct) =>
            {
                var existingEntries = await db.HashGetAllAsync(key);
                if (existingEntries.Any())
                {
                    Console.WriteLine($"Entity already exists for key {key}. Skipping update.");
                    return;
                }

                await db.HashSetAsync(key, hashEntries);
                await db.KeyExpireAsync(key, expiryTime);
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
                Console.WriteLine($"Error getting all hash values for key {hashKey}: {ex.Message}");
                return new Dictionary<string, string>();
            }
        }

        public async Task AddEntryToHashAsync(string key, string entryName, string entryValue, CancellationToken cancellationToken)
        {
            await AcquireLockAsync(key, async (ct) =>
            {
                var existingValue = await db.HashGetAsync(key, entryName);
                if (existingValue.HasValue)
                {
                    Console.WriteLine($"Data already exists for key {key} and id {entryName}. Skipping update.");
                    return;
                }

                await db.HashSetAsync(key, [new HashEntry(entryName, entryValue)]);
                await db.KeyExpireAsync(key, expiryTime);
            }, cancellationToken);
        }

        public async Task<string?> GetEntryValueFromHashAsync(string key, string entryName)
        {
            try
            {
                var value = await db.HashGetAsync(key, entryName);
                return value.HasValue ? value.ToString() : null;
            }
            catch (RedisException ex)
            {
                Console.WriteLine($"Error getting hash data for key {key} and field {entryName}: {ex.Message}");
                return null;
            }
        }

        public async Task AddValueToSetAsync(string key, string value, CancellationToken cancellationToken)
        {
            await AcquireLockAsync(key, async (ct) =>
            {
                var members = await db.SetMembersAsync(key);
                if (members.Contains(value))
                {
                    Console.WriteLine($"Item {value} already exists in index {key}. Skipping update.");
                    return;
                }

                await db.SetAddAsync(key, value);
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
                Console.WriteLine($"Error getting members of set with {key}: {ex.Message}");
                return new List<string>();
            }
        }

        public async Task AddStringAsync(string key, string value, CancellationToken cancellationToken)
        {
            await AcquireLockAsync(key, async (ct) =>
            {
                await db.StringSetAsync(key, value, expiryTime);
            }, cancellationToken);
        }

        public async Task<string?> GetStringAsync(string key)
        {
            try
            {
                return await db.StringGetAsync(key);
            }
            catch (RedisException ex)
            {
                Console.WriteLine($"Error getting value for key {key}: {ex.Message}");
                return null;
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

        private async Task<bool> AcquireLockAsync(
            string key, 
            Func<CancellationToken, Task> action, 
            CancellationToken cancellationToken = default, 
            int maxRetries = 5)
        {
            int threadId = Thread.CurrentThread.ManagedThreadId;
            int retries = 0;
            var waitTime = TimeSpan.FromSeconds(10);  // Maximum time to wait for the lock
            var retryTime = TimeSpan.FromMilliseconds(200);  // Interval between retries

            while (retries < maxRetries)
            {
                retries++;
                try
                {
                    await using (var redLock = await redLockFactory.CreateLockAsync(key, expiryTime, waitTime, retryTime, cancellationToken))
                    {
                        if (redLock.IsAcquired)
                        {
                            await action(cancellationToken);
                            return true;
                        }
                        // If the lock was not acquired, wait for retryTime and try again.
                    }
                }
                catch (RedisException ex)
                {
                    Console.WriteLine($"Thread {threadId} encountered Redis error performing action with key {key}: {ex.Message}");
                    return false;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Thread {threadId} encountered error performing action with key {key}: {ex.Message}");
                    return false;
                }
            }

            Console.WriteLine($"Thread {threadId} exhausted retries to acquire lock for key {key}.");
            return false;
        }
    }
}