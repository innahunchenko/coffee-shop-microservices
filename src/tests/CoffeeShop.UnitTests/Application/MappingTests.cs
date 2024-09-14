using AutoFixture;
using Catalog.Domain.Models;
using Catalog.Application.Mapping;
using FluentAssertions;
using Catalog.Domain.Models.Dtos;

namespace CoffeeShop.UnitTests.Application
{
    public class MappingTests
    {
        private readonly Fixture fixture;

        public MappingTests()
        {
            fixture = new Fixture();
        }

        [Theory]        
        [InlineData("Coffee", "WholeBean")]
        [InlineData("Coffee", null)]
        [InlineData(null, "WholeBean")]
        [InlineData(null, null)]
        public void ToProductDto_FromProduct_ShouldMapProperties(string parentCategoryName, string categoryName)
        {
            // Arrange
            var product = fixture.Build<Product>()
                .With(x => x.Category, 
                    CreateCategory(parentCategoryName, categoryName))
                .Create();

            // Act
            var productDto = product.ToProductDto();

            // Assert
            productDto.Should().BeEquivalentTo(new ProductDto
            {
                Id = product.Id.ToString(),
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                CategoryName = product.Category?.ParentCategory?.Name ?? product.Category?.Name ?? string.Empty,
                SubcategoryName = product.Category?.ParentCategory == null ? product.Category?.Name ?? string.Empty : string.Empty
            }, "ProductDto should match the mapped properties from Product");
        }

        [Fact]
        public void ToProductDto_FromDictionary_ShouldMapProperties()
        {
            // Arrange
            var productDto = fixture.Create<ProductDto>();
            var dictionary = new Dictionary<string, string>
            {
                { nameof(ProductDto.Id), productDto.Id },
                { nameof(ProductDto.Name), productDto.Name },
                { nameof(ProductDto.CategoryName), productDto.CategoryName },
                { nameof(ProductDto.SubcategoryName), productDto.SubcategoryName },
                { nameof(ProductDto.Description), productDto.Description },
                { nameof(ProductDto.Price), productDto.Price.ToString() }
            };

            // Act
            var mappedProductDto = dictionary.ToProductDto();

            // Assert
            mappedProductDto.Should().BeEquivalentTo(productDto, "ProductDto properties should match those from the dictionary");
        }

        [Fact]
        public void ToEntity_FromProductDto_ShouldMapProperties()
        {
            // Arrange
            var productDto = fixture.Create<ProductDto>();
            var expectedDictionary = new Dictionary<string, string>
            {
                { nameof(ProductDto.Id), productDto.Id ?? string.Empty },
                { nameof(ProductDto.Name), productDto.Name ?? string.Empty },
                { nameof(ProductDto.CategoryName), productDto.CategoryName ?? string.Empty },
                { nameof(ProductDto.SubcategoryName), productDto.SubcategoryName ?? string.Empty },
                { nameof(ProductDto.Description), productDto.Description ?? string.Empty },
                { nameof(ProductDto.Price), productDto.Price.ToString() }
            };

            // Act
            var dictionary = productDto.ToEntity();

            // Assert
            dictionary.Should().BeEquivalentTo(expectedDictionary, "Dictionary should match the mapped properties from ProductDto");
        }

        private Category CreateCategory(string parentCategoryName, string categoryName)
        {
            return new Category
            {
                Name = categoryName,
                ParentCategory = new Category() { Name = parentCategoryName }
            };
        }
    }
}
