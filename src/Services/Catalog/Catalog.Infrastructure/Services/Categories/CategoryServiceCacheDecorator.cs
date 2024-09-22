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

        public async Task<List<CategoryDto>> GetAllCategoriesAsync()
        {
            var cachedCategories = await cacheService.GetCategoriesFromCacheAsync();
            if (cachedCategories.Any())
            {
                return cachedCategories;
            }

            var categoriesFromDb = await categoryService.GetAllCategoriesAsync();
            if (categoriesFromDb.Any())
            {
                await cacheService.AddToCacheAsync(categoriesFromDb);
                return categoriesFromDb;
            }

            return new List<CategoryDto>();
        }
    }
}
