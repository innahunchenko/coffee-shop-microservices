using Catalog.Domain.Models.Dtos;
using Catalog.Domain.Services.Categories;

namespace Catalog.Infrastructure.Services.Categories
{
    public class CategoryServiceCacheDecorator : ICategoryService
    {
        private readonly ICategoryService decorated;
        private readonly ICategoryCacheService cacheService;

        public CategoryServiceCacheDecorator(ICategoryService decorated, ICategoryCacheService cacheService)
        {
            this.decorated = decorated;
            this.cacheService = cacheService;
        }

        public async Task<List<CategoryDto>> GetAllCategoriesAsync()
        {
            var cachedCategories = await cacheService.GetCategoriesFromCacheAsync();
            if (cachedCategories.Any())
            {
                return cachedCategories;
            }

            var categoriesFromDb = await decorated.GetAllCategoriesAsync();
            if (categoriesFromDb.Any())
            {
                await cacheService.AddToCacheAsync(categoriesFromDb);
                return categoriesFromDb;
            }

            return new List<CategoryDto>();
        }
    }
}
