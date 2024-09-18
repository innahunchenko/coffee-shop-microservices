using AutoFixture;
using Catalog.Domain.Models.Dtos;
using Catalog.Infrastructure.Services.Categories;
using Catalog.Infrastructure.Services.Products;
using FluentAssertions;
using Moq;
using Newtonsoft.Json;
using RedisCachingService;

namespace CoffeeShop.UnitTests.Infrastructure
{
    public class CacheServiceTests
    {
        private readonly Mock<IRedisCacheRepository> redisCacheRepositoryMock;
        private readonly ProductCacheService productCacheService;
        private readonly CategoryCacheService categoryCacheService;

        private readonly Fixture fixture;

        public CacheServiceTests() 
        {
            redisCacheRepositoryMock = new Mock<IRedisCacheRepository>();
            productCacheService = new ProductCacheService(redisCacheRepositoryMock.Object);
            categoryCacheService = new CategoryCacheService(redisCacheRepositoryMock.Object);
            fixture = new Fixture();
        }

        [Fact]
        public async Task GetProductsFromCacheAsync_ReturnProducts_WhenExistInCache()
        {
            var productsDto = new Dictionary<string, string>
            {
                { "Id", "1" },
                { "Name", "Product 1" }
            };
            var productsKeys = fixture.Build<string>().CreateMany(2).ToList();
            redisCacheRepositoryMock.Setup(r => r.GetValuesFromSetAsync(It.IsAny<string>())).ReturnsAsync(productsKeys);
            redisCacheRepositoryMock.Setup(r => r.GetEntityFromHashAsync(productsKeys.First())).ReturnsAsync(productsDto);

            var result = await productCacheService.GetProductsFromCacheAsync("someIndex");

            result.Should().NotBeNull();
            result.Should().ContainSingle();
            result.First().Id.Should().Be(productsDto["Id"]);
            result.First().Name.Should().Be(productsDto["Name"]);
        }

        [Fact]
        public async Task GetProductsFromCacheAsync_ShouldReturnEmptyList_WhenCacheIsEmpty()
        {
            redisCacheRepositoryMock.Setup(repo => repo.GetValuesFromSetAsync(It.IsAny<string>()))
                .ReturnsAsync(new List<string>());

            var result = await productCacheService.GetProductsFromCacheAsync("someIndex");

            Assert.Empty(result);
        }

        [Fact]
        public async Task AddProductsToCacheAsync_ShouldAddProductsToCache_WhenCalled()
        {
            var products = new List<ProductDto>
            {
                new ProductDto { Id = "1", Name = "Product 1" },
                new ProductDto { Id = "2", Name = "Product 2" }
            };
            redisCacheRepositoryMock.Setup(repo => repo.AddEntityToHashAsync(It.IsAny<string>(), It.IsAny<Dictionary<string, string>>()))
                .Returns(Task.FromResult(true));

            await productCacheService.AddProductsToCacheAsync(products);

            redisCacheRepositoryMock.Verify(repo => repo.AddEntityToHashAsync(It.Is<string>(key => key == "product:1"), 
                It.IsAny<Dictionary<string, string>>()), Times.Once);
            redisCacheRepositoryMock.Verify(repo => repo.AddEntityToHashAsync(It.Is<string>(key => key == "product:2"), 
                It.IsAny<Dictionary<string, string>>()), Times.Once);
        }

        [Fact]
        public async Task AddProductsToIndexAsync_AddProductsToIndex_WhenCalled()
        {
            var products = new List<ProductDto>
            {
                new ProductDto { Id = "1", Name = "Product 1" },
                new ProductDto { Id = "2", Name = "Product 2" }
            };
            redisCacheRepositoryMock.Setup(e => e.AddValueToSetAsync(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.FromResult(true));

            await productCacheService.AddProductsToIndexAsync("productIndex", products);

            redisCacheRepositoryMock.Verify(r => r.AddValueToSetAsync("productIndex", "product:1"), Times.Once);
            redisCacheRepositoryMock.Verify(r => r.AddValueToSetAsync("productIndex", "product:2"), Times.Once);
        }

        [Fact]
        public async Task GetCachedTotalProductsCountAsync_ShouldReturnTotalCount_WhenCacheHasData()
        {
            redisCacheRepositoryMock.Setup(repo => repo.GetStringAsync(It.IsAny<string>()))
                .ReturnsAsync("100");

            var result = await productCacheService.GetCachedTotalProductsCountAsync("totalKey");

            Assert.Equal(100, result);
        }


        [Fact]
        public async Task AddTotalProductsCountToCacheAsync_ShouldAddTotalCountToCache_WhenCalled()
        {
            redisCacheRepositoryMock.Setup(repo => repo.AddStringAsync(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.FromResult(true));

            await productCacheService.AddTotalProductsCountToCacheAsync("totalKey", 100);

            redisCacheRepositoryMock.Verify(repo => repo.AddStringAsync("totalKey", "100"), Times.Once);
        }

        [Fact]
        public async Task GetCategoriesFromCacheAsync_ReturnCategories_WhenExistInCache()
        {
            var category = fixture.Build<CategoryDto>().Create();
            var categories = new Dictionary<string, string>
            {
                { category.Name, JsonConvert.SerializeObject(category) }
            };

            redisCacheRepositoryMock.Setup(r => r.GetEntityFromHashAsync(It.IsAny<string>())).ReturnsAsync(categories);

            var result = await categoryCacheService.GetFromCacheAsync();

            result.Should().NotBeNull();
            result.First().Name.Should().Be(category.Name);
        }

        [Fact]
        public async Task GetCategoriesFromCacheAsync_ShouldReturnEmptyList_WhenCacheIsEmpty()
        {
            redisCacheRepositoryMock.Setup(repo => repo.GetEntityFromHashAsync(It.IsAny<string>()))
                .ReturnsAsync(new Dictionary<string, string>());

            var result = await categoryCacheService.GetFromCacheAsync();

            Assert.Empty(result);
        }

        [Fact]
        public async Task AddCategoriesToCacheAsync_ShouldAddCategoriesToCache_WhenCalled()
        {
            var categoryKey = "KEY:CATEGORIES";
            var categories = new List<CategoryDto>
            {
                new CategoryDto { Name = "Category 1" },
                new CategoryDto { Name = "Category 2" }
            };

            redisCacheRepositoryMock.Setup(repo => repo.AddEntityToHashAsync(It.IsAny<string>(), It.IsAny<Dictionary<string, string>>()))
                .Returns(Task.FromResult(true));

            await categoryCacheService.AddToCacheAsync(categories);

            redisCacheRepositoryMock.Verify(repo => repo.AddEntityToHashAsync(It.Is<string>(key => key == categoryKey),
                It.IsAny<Dictionary<string, string>>()), Times.Once);
        }
    }
}
