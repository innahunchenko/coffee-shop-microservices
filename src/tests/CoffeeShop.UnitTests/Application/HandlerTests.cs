using Catalog.Application.Categories.Queries;
using Catalog.Application.Products.Queries;
using Catalog.Domain.Models;
using Catalog.Domain.Models.Dtos;
using Catalog.Domain.Models.Pagination;
using Catalog.Domain.Services.Categories;
using Catalog.Domain.Services.Products;
using FluentAssertions;
using Moq;

namespace CoffeeShop.UnitTests.Application
{
    public class HandlerTests
    {
        [Fact]
        public async Task Products_Handle_ValidRequest_ReturnsPaginatedList()
        {
            // Arrange
            var mockProductService = new Mock<IProductService>();
            var pageNumber = 1;
            var pageSize = 8;
            var totalCount = 25;
            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
            var request = new GetAllProductsRequest(pageNumber, pageSize);
            var products = new List<ProductDto>
            {
                new ProductDto(),
                new ProductDto()
            };

            mockProductService.Setup(service => service.GetAllProductsAsync(It.IsAny<PaginationParameters>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new PaginatedList<ProductDto>(products, totalCount, pageSize));
            var handler = new GetAllProductsHandler(mockProductService.Object);

            // Act
            var result = await handler.Handle(request, CancellationToken.None);

            // Assert
            result.Should().NotBeNull("because the handler should return a result");
            result.Items.Count().Should().Be(products.Count, "because the number of items should match the products list");
            result.TotalPages.Should().Be(totalPages, "because the total pages should be correctly calculated");
            result.TotalCount.Should().Be(totalCount, "because the total count should match the expected value");
        }

        [Fact]
        public async Task Categories_Handle_ValidRequest_ReturnsCategoriesList()
        {
            // Arrange
            var mockCategoriesService = new Mock<ICategoryService>();
            var request = new GetAllCategoriesRequest();
            var categoories = new List<CategoryDto>
            {
                new CategoryDto(),
                new CategoryDto()
            };

            mockCategoriesService.Setup(service => service.GetCategoriesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<CategoryDto>(categoories));
            var handler = new GetAllCategoriesHandler(mockCategoriesService.Object);

            // Act
            var result = await handler.Handle(request, CancellationToken.None);

            // Assert
            result.Should().NotBeNull("because the handler should return a result");
            result.Count().Should().Be(categoories.Count, "because the number of items should match the categories list");
        }
    }
}
