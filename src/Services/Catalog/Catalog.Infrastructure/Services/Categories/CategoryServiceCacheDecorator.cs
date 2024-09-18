using Catalog.Domain.Models.Dtos;
using Catalog.Domain.Services.Categories;
using Microsoft.Extensions.Logging;

namespace Catalog.Infrastructure.Services.Categories
{
    public class CategoryServiceCacheDecorator : ICategoryService
    {
        private readonly ICategoryService categoryService;
        private readonly ICategoryCacheService cacheService;
        private readonly ILogger<CategoryServiceCacheDecorator> logger;

        public CategoryServiceCacheDecorator(ICategoryService categoryService, 
            ICategoryCacheService cacheService, ILogger<CategoryServiceCacheDecorator> logger)
        {
            this.categoryService = categoryService;
            this.cacheService = cacheService;
            this.logger = logger;
        }

        public async Task<List<CategoryDto>> GetAsync()
        {
            var cachedCategories = await cacheService.GetFromCacheAsync();
            if (cachedCategories.Any())
            {
                logger.LogInformation($"{cachedCategories.Count} categories retrieved from cache");
                return cachedCategories;
            }

            var categoriesFromDb = await categoryService.GetAsync();
            if (categoriesFromDb.Any())
            {
                await cacheService.AddToCacheAsync(categoriesFromDb);
                logger.LogInformation($"{categoriesFromDb.Count} categories added to cache");
                return categoriesFromDb;
            }

            return new List<CategoryDto>();
        }
    }
}
