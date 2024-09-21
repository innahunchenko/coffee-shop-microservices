using Catalog.Domain.Models.Dtos;

namespace Catalog.Domain.Repositories.Categories
{
    public interface ICategoryRepository
    {
        Task<List<CategoryDto>> GetAllCategoriesAsync();
    }
}
