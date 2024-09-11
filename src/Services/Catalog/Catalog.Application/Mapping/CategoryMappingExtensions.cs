using Catalog.Domain.Models;
using Catalog.Domain.Models.Dtos;

namespace Catalog.Application.Mapping
{
    public static class CategoryMappingExtensions
    {
        public static CategoryDto ToCategoryDto(this Category src)
        {
            return new CategoryDto
            {
                Name = src.Name,
                Subcategories = src.Subcategories.Select(sc => sc.Name).ToList()
            };
        }
    }
}
