using Catalog.Domain.Models;
using Catalog.Domain.Repositories.Categories;
using Catalog.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Infrastructure.Repositories.Categories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly AppDbContext context;

        public CategoryRepository(AppDbContext context)
        {
            this.context = context;
        }

        public async Task<List<Category>> GetMainCategoriesWithSubcategoriesAsync()
        {
            return await context.Categories
                .Where(c => c.ParentCategoryId == null)
                .Include(c => c.Subcategories)
                .ToListAsync();
        }
    }
}
