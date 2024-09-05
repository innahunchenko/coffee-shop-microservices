using Catalog.Domain.Models.Dtos;

namespace Catalog.Domain.Services.Categories
{
    public interface ICategoryCacheService
    {
        Task<List<CategoryDto>> GetCategoriesFromCacheAsync();
        Task AddCategoriesToCacheAsync(List<CategoryDto> categories, CancellationToken cancellationToken);
    }
}
