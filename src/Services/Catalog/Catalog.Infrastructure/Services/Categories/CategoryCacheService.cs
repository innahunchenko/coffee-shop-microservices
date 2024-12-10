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

        public async Task AddOrUpdateCategoryInCacheAsync(string name, string? parentCategoryName)
        {
            var categories = await GetCategoriesFromCacheAsync();

            var index = categories.FindIndex(c => c.Name == parentCategoryName);

            if (index >= 0)
            {
                categories[index].Subcategories.Add(name);
            }
            else
            {
                var category = new CategoryDto { Name = name, Subcategories = new List<string>() };
                categories.Add(category);
            }

            await ReloadCacheAsync(categories);
            logger.LogInformation($"Category '{name}' added/updated in cache.");
        }

        public async Task RemoveCategoryFromCacheAsync(string categoryName)
        {
            var categories = await GetCategoriesFromCacheAsync();

            var parentCategoryForSubcategory = categories.FirstOrDefault(c => c.Subcategories.Contains(categoryName));

            if (parentCategoryForSubcategory != null)
            {
                parentCategoryForSubcategory.Subcategories.Remove(categoryName);
                logger.LogInformation($"Subcategory '{categoryName}' removed from parent category '{parentCategoryForSubcategory.Name}' in cache.");

                var index = categories.FindIndex(c => c.Name == parentCategoryForSubcategory.Name);
                if (index != -1)
                {
                    categories[index] = parentCategoryForSubcategory;
                }

                await ReloadCacheAsync(categories);
                return;
            }

            var categoryToRemove = categories.FirstOrDefault(c => c.Name == categoryName);

            if (categoryToRemove != null)
            {
                categories.Remove(categoryToRemove);
                logger.LogInformation($"Category '{categoryName}' removed from cache.");
                await ReloadCacheAsync(categories);
                return;
            }

            logger.LogWarning($"Category '{categoryName}' not found in cache.");
        }

        public async Task ReloadCacheAsync(List<CategoryDto> categories)
        {
            var serializedCategories = categories.ToDictionary(c => c.Name, JsonConvert.SerializeObject);
            var result = await cacheRepository.AddEntityToHashAsync(CATEGORIES_KEY, serializedCategories);
        }
    }
}
