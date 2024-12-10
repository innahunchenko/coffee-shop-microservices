using Catalog.Domain.Models.Dtos;

namespace Catalog.Domain.Services.Categories
{
    public interface ICategoryService
    {
        Task<List<CategoryDto>> GetAllCategoriesAsync();
        Task<Guid> AddCategoryAsync(string name, string? parentCategoryName);
        Task UpdateCategoryAsync(string oldName, string newName, string? parentCategoryName);
        Task DeleteCategoryAsync(string categoryName);
    }
}
