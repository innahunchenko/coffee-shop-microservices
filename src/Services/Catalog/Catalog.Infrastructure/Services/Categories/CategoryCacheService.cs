using RedisCachingService;
using Catalog.Domain.Services.Categories;
using Catalog.Domain.Models.Dtos;
using Newtonsoft.Json;

namespace Catalog.Infrastructure.Services.Categories
{
    public class CategoryCacheService : ICategoryCacheService
    {
        private readonly IRedisCacheRepository cacheRepository;

        private static readonly string CATEGORIES_KEY = "KEY:CATEGORIES";

        public CategoryCacheService(IRedisCacheRepository cacheRepository)
        {
            this.cacheRepository = cacheRepository;
        }

        public async Task<List<CategoryDto>> GetFromCacheAsync()
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

            return categories;
        }

        public async Task AddToCacheAsync(List<CategoryDto> categories)
        {
            var cachedCategries = categories.ToDictionary(category => category.Name, JsonConvert.SerializeObject);
            await cacheRepository.AddEntityToHashAsync(CATEGORIES_KEY, cachedCategries);
        }
    }
}
