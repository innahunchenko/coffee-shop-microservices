using AutoFixture;
using Catalog.Domain.Models.Dtos;
using Catalog.Domain.Models.Pagination;
using Catalog.Domain.Services.Categories;
using Catalog.Domain.Services.Products;
using Catalog.Infrastructure.Services.Categories;
using Catalog.Infrastructure.Services.Products;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;

namespace CoffeeShop.UnitTests.Infrastructure
{
    public class ServiceCacheDecoratorTests
    {
        private readonly Mock<IProductCacheService> productCacheServiceMock;
        private readonly ProductServiceCacheDecorator productServiceCacheDecorator;
        private readonly Mock<IProductService> productServiceMock;
        private readonly Mock<ICategoryCacheService> categoryCacheServiceMock;
        private readonly CategoryServiceCacheDecorator categoryServiceCacheDecorator;
        private readonly Mock<ICategoryService> categoryServiceMock;
        private readonly Fixture fixture;

        public ServiceCacheDecoratorTests()
        {
            productCacheServiceMock = new Mock<IProductCacheService>();
            productServiceMock = new Mock<IProductService>();
            productServiceCacheDecorator = new ProductServiceCacheDecorator(productServiceMock.Object, productCacheServiceMock.Object);

            categoryCacheServiceMock = new Mock<ICategoryCacheService>();
            categoryServiceMock = new Mock<ICategoryService>();
            categoryServiceCacheDecorator = new CategoryServiceCacheDecorator(categoryServiceMock.Object, categoryCacheServiceMock.Object);

            fixture = new Fixture();
        }

        [Fact]
        public async Task GetAllProductsAsync_ShouldReturnFromCache_WhenCacheIsAvailable()
        {
            var paginationParameters = new PaginationParameters(1, 8);
            var expectedCachedProduct = fixture.Build<ProductDto>().CreateMany(2).ToList();
            var expectedTotalCount = 100;
            productCacheServiceMock.Setup(c => c.GetProductsFromCacheAsync(It.IsAny<string>())).ReturnsAsync(expectedCachedProduct);
            productCacheServiceMock.Setup(c => c.GetCachedTotalProductsCountAsync(It.IsAny<string>())).ReturnsAsync(expectedTotalCount);

            var cachedResult = await productServiceCacheDecorator.GetAllProductsAsync(paginationParameters);

            cachedResult.Should().NotBeNull("because the product cache decorator should return a cached result");
            cachedResult.Items.Count().Should().Be(expectedCachedProduct.Count(), "because the number of cached items should match the products list");
            cachedResult.TotalCount.Should().Be(expectedTotalCount, "because the cached total count products should match the expected value");
            productServiceMock.Verify(s => s.GetAllProductsAsync(It.IsAny<PaginationParameters>()), Times.Never);  
        }

        [Fact]
        public async Task GetAllCategoriesAsync_ShouldReturnFromCache_WhenCacheIsAvailable()
        {
            var expectedCachedCategories = fixture.Build<CategoryDto>().CreateMany(2).ToList();
            categoryCacheServiceMock.Setup(c => c.GetCategoriesFromCacheAsync()).ReturnsAsync(expectedCachedCategories);

            var cachedResult = await categoryServiceCacheDecorator.GetAllCategoriesAsync();

            cachedResult.Should().NotBeNull("because the category cache decorator should return a cached result");
            cachedResult.Count().Should().Be(expectedCachedCategories.Count(), "because the number of cached categories should match the expected categories list");
            categoryServiceMock.Verify(s => s.GetAllCategoriesAsync(), Times.Never);
        }
    }
}
