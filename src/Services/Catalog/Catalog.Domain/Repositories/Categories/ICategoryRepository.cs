using Catalog.Domain.Models.Dtos;

namespace Catalog.Domain.Repositories.Categories
{
    public interface ICategoryRepository
    {
        Task<List<CategoryDto>> GetAllCategoriesAsync();
        Task<Guid> AddCategoryAsync(string name, string? parentCategoryName);
        Task UpdateCategoryAsync(string oldName, string newName, string? newParentCategoryName);
        Task DeleteCategoryAsync(string categoryName);
    }
}
