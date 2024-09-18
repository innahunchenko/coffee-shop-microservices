using Catalog.Domain.Models.Dtos;

namespace Catalog.Domain.Services.Categories
{
    public interface ICategoryCacheService
    {
        Task<List<CategoryDto>> GetFromCacheAsync();
        Task AddToCacheAsync(List<CategoryDto> categories);
    }
}
