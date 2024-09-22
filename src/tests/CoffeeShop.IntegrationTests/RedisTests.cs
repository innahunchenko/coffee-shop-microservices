using FluentAssertions;
using RedisCachingService;
using StackExchange.Redis;

namespace CoffeeShop.IntegrationTests
{
    public class RedisTests
    {
        private readonly RedisCacheRepository redisCacheRepository;

        public RedisTests()
        {
            var connectionMultiplexer = ConnectionMultiplexer.Connect("localhost:6379");
            var expiryTime = TimeSpan.FromMinutes(5);
            redisCacheRepository = new RedisCacheRepository(connectionMultiplexer, expiryTime);
        }

        [Fact]
        public async Task AddValueToSetAsync_MultipleThreads_ShouldAddAllValues()
        {
            var key = "test_set";
            var values = Enumerable.Range(1, 10).Select(i => i.ToString()).ToList();
            var tasks = new List<Task>();

            foreach (var value in values)
            {
                tasks.Add(Task.Run(async () =>
                {
                    await redisCacheRepository.AddValueToSetAsync(key, value);
                }));
            }

            await Task.WhenAll(tasks);

            var result = await redisCacheRepository.GetValuesFromSetAsync(key);
            result.Count.Should().Be(10, $"{result.Count} should be 10");
            foreach (var value in values)
            {
                result.Contains(value).Should().BeTrue($"Value {value} was not found in the set.");
            }
        }

        [Fact]
        public async Task AcquireLock_ShouldPreventConcurrentExecution()
        {
            // Arrange
            var key = "lockkey";
            var expectedValue = "lockedvalue";
            var successfulSetOperations = 0;

            async Task<bool> TrySetValueAsync(CancellationToken ct)
            {
                var result = await redisCacheRepository.AddValueToSetAsync(key, expectedValue);
                if (result)
                {
                    Interlocked.Increment(ref successfulSetOperations);
                }
                return result;
            }

            // Act
            var tasks = Enumerable.Range(0, 10).Select(_ => Task.Run(() => TrySetValueAsync(CancellationToken.None)));
            await Task.WhenAll(tasks);

            // Assert
            var values = await redisCacheRepository.GetValuesFromSetAsync(key);
            Assert.Single(values);  // Ensure that only one unique value is present
            Assert.Equal(expectedValue, values.Single());
            Assert.Equal(1, successfulSetOperations);  // Ensure only one successful set operation
        }
    }
}