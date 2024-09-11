using Catalog.Domain.Models;
using Catalog.Domain.Models.Dtos;

namespace Catalog.Application.Mapping
{
    public static class ProductMappingExtensions
    {
        public static ProductDto ToProductDto(this Product src)
        {
            return new ProductDto
            {
                Id = src.Id.ToString(),
                Name = src.Name,
                Description = src.Description,
                Price = src.Price,
                CategoryName = src.Category != null
                    ? (src.Category.ParentCategory != null
                        ? src.Category.ParentCategory.Name
                        : src.Category.Name)
                    : string.Empty,
                SubcategoryName = src.Category != null 
                    ? (src.Category.ParentCategory == null 
                        ? src.Category.Name 
                        : string.Empty) 
                    : string.Empty
            };
        }

        public static ProductDto ToProductDto(this IDictionary<string, string> src)
        {
            return new ProductDto
            {
                Id = src.TryGetValue(nameof(ProductDto.Id), out var id) ? id : string.Empty,
                Name = src.TryGetValue(nameof(ProductDto.Name), out var name) ? name : string.Empty,
                CategoryName = src.TryGetValue(nameof(ProductDto.CategoryName), out var categoryName) ? categoryName : string.Empty,
                SubcategoryName = src.TryGetValue(nameof(ProductDto.SubcategoryName), out var subcategoryName) ? subcategoryName : string.Empty,
                Description = src.TryGetValue(nameof(ProductDto.Description), out var description) ? description : string.Empty,
                Price = src.TryGetValue(nameof(ProductDto.Price), out var price) && decimal.TryParse(price, out var parsedPrice) ? parsedPrice : 0m
            };
        }

        public static IDictionary<string, string> ToEntity(this ProductDto src)
        {
            var dictionary = new Dictionary<string, string>
            {
                { nameof(ProductDto.Id), src.Id ?? string.Empty },
                { nameof(ProductDto.Name), src.Name ?? string.Empty },
                { nameof(ProductDto.CategoryName), src.CategoryName ?? string.Empty },
                { nameof(ProductDto.SubcategoryName), src.SubcategoryName ?? string.Empty },
                { nameof(ProductDto.Description), src.Description ?? string.Empty },
                { nameof(ProductDto.Price), src.Price.ToString() }
            };

            return dictionary;
        }
    }
}
