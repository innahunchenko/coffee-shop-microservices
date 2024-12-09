using Catalog.Domain.Models.Dtos;

namespace Catalog.Domain.Services.Categories
{
    public interface ICategoryService
    {
        Task<List<CategoryDto>> GetAllCategoriesAsync();
        Task<Guid> AddCategoryAsync(string name, string? parentCategoryName);
        Task UpdateCategoryAsync(string oldName, string newName);
        Task DeleteCategoryAsync(string categoryName);
        Task<CategoryDto> GetCategoryByNameAsync(string name);
    }
}
