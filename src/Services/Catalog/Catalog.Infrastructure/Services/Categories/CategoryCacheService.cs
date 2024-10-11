using RedisCachingService;
using Catalog.Domain.Services.Categories;
using Catalog.Domain.Models.Dtos;
using Newtonsoft.Json;
using Microsoft.Extensions.Logging;

namespace Catalog.Infrastructure.Services.Categories
{
    public class CategoryCacheService : ICategoryCacheService
    {
        private readonly IRedisCacheRepository cacheRepository;
        private readonly ILogger<CategoryCacheService> logger;

        private static readonly string CATEGORIES_KEY = "KEY:CATEGORIES";
        private static readonly string FirstCategory = "Coffee";

        public CategoryCacheService(IRedisCacheRepository cacheRepository, ILogger<CategoryCacheService> logger)
        {
            this.cacheRepository = cacheRepository;
            this.logger = logger;
        }

        public async Task<List<CategoryDto>> GetCategoriesFromCacheAsync()
        {
            var cachedValues = await cacheRepository.GetEntityFromHashAsync(CATEGORIES_KEY);
            var categories = new List<CategoryDto>();

            if (!cachedValues.Any())
            {
                return categories;
            }

            foreach (var item in cachedValues)
            {
                var categoryDto = JsonConvert.DeserializeObject<CategoryDto>(item.Value);
                categories.Add(categoryDto!);
            }

            categories = categories
                .OrderBy(c => c.Name == FirstCategory ? 0 : 1)
                .ThenBy(c => c.Name)
                .ThenBy(c => c.Subcategories != null ? c.Subcategories.FirstOrDefault() : string.Empty)
                .ToList();

            logger.LogInformation("All cached categories retrieved from cache");

            return categories;
        }

        public async Task AddToCacheAsync(List<CategoryDto> categories)
        {
            var cachedCategries = categories.ToDictionary(category => category.Name, JsonConvert.SerializeObject);
            var result = await cacheRepository.AddEntityToHashAsync(CATEGORIES_KEY, cachedCategries);
            logger.LogInformation($"All categories {(result ? "added" : "has not been added")} to cache");
        }
    }
}
