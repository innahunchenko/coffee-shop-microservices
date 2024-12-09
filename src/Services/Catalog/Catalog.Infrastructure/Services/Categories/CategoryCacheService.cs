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
            var cachedData = await cacheRepository.GetEntityFromHashAsync(CATEGORIES_KEY);
            var categories = new List<CategoryDto>();

            if (!cachedData.Any())
            {
                return categories;
            }

            foreach (var item in cachedData)
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

        public async Task AddOrUpdateCategoryInCacheAsync(CategoryDto category)
        {
            var categories = await GetCategoriesFromCacheAsync();

            var existingCategory = categories.FirstOrDefault(c => c.Name == category.Name);
            if (existingCategory != null)
            {
                existingCategory.Subcategories = category.Subcategories;
            }
            else
            {
                categories.Add(category);
            }

            await ReloadCacheAsync(categories);
            logger.LogInformation($"Category '{category.Name}' added/updated in cache.");
        }

        public async Task RemoveCategoryFromCacheAsync(string categoryName)
        {
            var categories = await GetCategoriesFromCacheAsync();

            var categoryToRemove = categories.FirstOrDefault(c => c.Name == categoryName);
            if (categoryToRemove != null)
            {
                categories.Remove(categoryToRemove);
                await ReloadCacheAsync(categories);
                logger.LogInformation($"Category '{categoryName}' removed from cache.");
            }
            else
            {
                logger.LogWarning($"Category '{categoryName}' not found in cache.");
            }
        }

        public async Task ReloadCacheAsync(List<CategoryDto> categories)
        {
            var serializedCategories = categories.ToDictionary(c => c.Name, JsonConvert.SerializeObject);
            var result = await cacheRepository.AddEntityToHashAsync(CATEGORIES_KEY, serializedCategories);

           // logger.LogInformation($"All categories {(result ? "added" : "has not been added")} to cache");
        }

        public async Task<CategoryDto?> GetCategoryByNameFromCacheAsync(string name)
        {
            var cachedData = await cacheRepository.GetEntityFromHashAsync(CATEGORIES_KEY);
            var categories = new List<CategoryDto>();

            if (!cachedData.Any())
            {
                return null;
            }

            foreach (var item in cachedData)
            {
                var categoryDto = JsonConvert.DeserializeObject<CategoryDto>(item.Value);
                categories.Add(categoryDto!);
            }

            var category = categories.FirstOrDefault(c => c.Name == name);

            if (category == null)
            {
                logger.LogWarning($"Category with Name {name} not found in cache.");
                return null;
            }

            logger.LogInformation($"Category with Name {name} retrieved from cache.");

            return category;
        }
    }
}
