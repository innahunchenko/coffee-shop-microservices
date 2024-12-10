using Catalog.Domain.Models.Dtos;

namespace Catalog.Domain.Services.Categories
{
    public interface ICategoryCacheService
    {
        Task<List<CategoryDto>> GetCategoriesFromCacheAsync();
        Task ReloadCacheAsync(List<CategoryDto> categories);
        Task AddOrUpdateCategoryInCacheAsync(string name, string? parentCategoryName);
        Task RemoveCategoryFromCacheAsync(string categoryName);
    }
}
