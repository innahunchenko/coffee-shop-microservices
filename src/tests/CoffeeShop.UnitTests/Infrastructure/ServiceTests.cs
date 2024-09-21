using AutoFixture;
using Catalog.Domain.Models.Dtos;
using Catalog.Domain.Models.Pagination;
using Catalog.Domain.Repositories.Categories;
using Catalog.Domain.Repositories.Products;
using Catalog.Infrastructure.Services.Categories;
using Catalog.Infrastructure.Services.Products;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;

namespace CoffeeShop.UnitTests.Infrastructure
{
    public class ServiceTests
    {
        private readonly Mock<IProductRepository> productRepositoryMock;
        private readonly ProductService productService;
        private readonly Mock<ICategoryRepository> categoryRepositoryMock;
        private readonly CategoryService categoryService;
        private readonly Fixture fixture;

        public ServiceTests() 
        {
            productRepositoryMock = new Mock<IProductRepository>();
            var loggerProductMock = new Mock<ILogger<ProductService>>();
            productService = new ProductService(productRepositoryMock.Object, loggerProductMock.Object);
            categoryRepositoryMock = new Mock<ICategoryRepository>();
            var loggerCategoryMock = new Mock<ILogger<CategoryService>>();
            categoryService = new CategoryService(categoryRepositoryMock.Object, loggerCategoryMock.Object);
            fixture = new Fixture();
            fixture.Behaviors.OfType<ThrowingRecursionBehavior>()
                .ToList()
                .ForEach(b => fixture.Behaviors.Remove(b));
            fixture.Behaviors.Add(new OmitOnRecursionBehavior());
        }

        [Fact]
        public async Task GetAllProductsAsync_ShouldReturnPaginatedList_WhenProductsExist()
        {
            // Arrange
            var paginationParameters = new PaginationParameters(1, 8);
            var products = fixture.Build<ProductDto>().CreateMany(2).ToList();
            var expectedTotalCountProducts = 1;
            productRepositoryMock.Setup(p => p.GetAllProductsAsync(paginationParameters)).ReturnsAsync(products);
            productRepositoryMock.Setup(p => p.GetAllProductsTotalCountAsync()).ReturnsAsync(expectedTotalCountProducts);

            // Act
            var productsDto = await productService.GetAllProductsAsync(paginationParameters);

            // Assert
            productsDto.Should().NotBeNull("because the product service should return a result");
            productsDto.Items.Count().Should().Be(products.Count(), "because the number of items should match the products list");
            productsDto.TotalCount.Should().Be(expectedTotalCountProducts, "because the total count products should match the expected value");
        }

        [Fact]
        public async Task GetAllCategoriesAsync_ShouldReturnList_WhenCategoriesExist()
        {
            // Arrange
            var categories = fixture.Build<CategoryDto>().CreateMany(2).ToList();
            categoryRepositoryMock.Setup(c => c.GetAllCategoriesAsync()).ReturnsAsync(categories);

            // Act
            var categoiesDto = await categoryService.GetAllCategoriesAsync();

            // Assert
            categoiesDto.Should().NotBeNull("because the category service should return a result");
            categoiesDto.Count().Should().Be(categories.Count(), "because the number of items should match the categories list");
        }
    }
}
