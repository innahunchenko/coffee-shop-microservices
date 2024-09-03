using Catalog.Domain.Models;

namespace Catalog.Domain.Repositories.Categories
{
    public interface ICategoryRepository
    {
        Task<List<Category>> GetMainCategoriesWithSubcategoriesAsync();
    }
}
