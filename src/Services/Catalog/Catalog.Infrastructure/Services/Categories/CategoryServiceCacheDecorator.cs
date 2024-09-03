using Catalog.Domain.Models.Dtos;
using Catalog.Domain.Services.Categories;

namespace Catalog.Infrastructure.Services.Categories
{
    public class CategoryServiceCacheDecorator : ICategoryService
    {
        private readonly ICategoryService categoryService;
        private readonly ICategoryCacheService cacheService;

        public CategoryServiceCacheDecorator(ICategoryService categoryService, ICategoryCacheService cacheService)
        {
            this.categoryService = categoryService;
            this.cacheService = cacheService;
        }

        public async Task<List<CategoryDto>> GetCategoriesAsync()
        {
            var cachedCategories = await cacheService.GetCategoriesFromCacheAsync();
            if (cachedCategories.Any())
            {
                Console.WriteLine($"Categories from cache");
                return cachedCategories;
            }

            var categoriesFromDb = await categoryService.GetCategoriesAsync();
            if (categoriesFromDb.Any())
            {
                await cacheService.AddCategoriesToCacheAsync(categoriesFromDb);
                Console.WriteLine($"categories from db");
                return categoriesFromDb;
            }

            return new List<CategoryDto>();
        }
    }
}
