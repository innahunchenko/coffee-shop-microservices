using Catalog.Domain.Models;
using Catalog.Domain.Models.Dtos;
using Mapster;

namespace Catalog.Application.Mapping
{
    public static class MapsterConfiguration
    {
        public static TypeAdapterConfig Configure()
        {
            var config = new TypeAdapterConfig();

            config.NewConfig<Category, CategoryDto>()
                .Map(dest => dest.Name, src => src.Name)
                .Map(dest => dest.Subcategories, src => src.Subcategories.Select(sc => sc.Name).ToList());

            config.NewConfig<CategoryDto, Category>();

            config.NewConfig<Product, ProductDto>()
                .Map(dest => dest.Id, src => src.Id.ToString()) 
                .Map(dest => dest.CategoryName,
                     src => src.Category != null
                         ? (src.Category.ParentCategory != null
                             ? src.Category.ParentCategory.Name
                             : src.Category.Name)
                         : string.Empty)
                .Map(dest => dest.SubcategoryName,
                     src => src.Category != null
                         ? (src.Category.ParentCategory == null
                             ? src.Category.Name
                             : string.Empty)
                         : string.Empty);

            //config.NewConfig<ProductDto, Product>()
            //    .Map(dest => dest.Id, src => Guid.Parse(src.Id))
            //    .Map(dest => dest.CategoryId, src => src.Category.Id != null ? Guid.Parse(src.CategoryId) : (Guid?)null);

            config.NewConfig<ProductDto, IDictionary<string, string>>()
                .MapWith(src => MapFromProductDto(src));

            config.NewConfig<IDictionary<string, string>, ProductDto>()
                .MapWith(src => MapToProductDto(src));

            return config;
        }

        public static ProductDto MapToProductDto(IDictionary<string, string> src)
        {
            var productDto = new ProductDto();

            foreach (var property in typeof(ProductDto).GetProperties())
            {
                if (src.TryGetValue(property.Name, out var valueString) && !string.IsNullOrEmpty(valueString))
                {
                    var value = Convert.ChangeType(valueString, property.PropertyType);
                    property.SetValue(productDto, value);
                }
            }

            return productDto;
        }

        public static IDictionary<string, string> MapFromProductDto(ProductDto src)
        {
            var dictionary = new Dictionary<string, string>();

            foreach (var property in typeof(ProductDto).GetProperties())
            {
                var value = property.GetValue(src);
                if (value != null)
                {
                    dictionary[property.Name] = value.ToString()!;
                }
            }

            return dictionary;
        }
    }
}
