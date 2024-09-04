using Catalog.Domain.Models.Dtos;
using Catalog.Domain.Models.Pagination;
using Catalog.Domain.Models;
using MapsterMapper;
using RedisCachingService;
using Catalog.Domain.Services.Products;

namespace Catalog.Infrastructure.Services.Products
{
    public class ProductCacheService : IProductCacheService
    {
        private readonly IRedisCacheRepository cacheRepository;
        private readonly IMapper mapper;

        public ProductCacheService(IRedisCacheRepository cacheRepository, IMapper mapper)
        {
            this.cacheRepository = cacheRepository;
            this.mapper = mapper;
        }

        public async Task<PaginatedList<ProductDto>> GetProductsFromCacheAsync(string index, string totalKey, PaginationParameters paginationParameters)
        {
            var productsKeys = await cacheRepository.GetValuesFromSetAsync(index);

            var cachedProducts = new List<ProductDto>();

            foreach (var key in productsKeys)
            {
                var entryDictionary = await cacheRepository.GetEntityFromHashAsync(key);
                if (entryDictionary != null && entryDictionary.Count > 0)
                {
                    var productDto = mapper.Map<ProductDto>(entryDictionary);
                    cachedProducts.Add(productDto);
                }
            }

            if (!cachedProducts.Any())
            {
                return new PaginatedList<ProductDto>(new List<ProductDto>(), 0, paginationParameters.PageSize);
            }

            var cachedTotalProducts = await cacheRepository.GetStringAsync(totalKey);
            int totalProducts = string.IsNullOrEmpty(cachedTotalProducts) ? 0 : int.Parse(cachedTotalProducts);

            return new PaginatedList<ProductDto>(cachedProducts, totalProducts, paginationParameters.PageSize);
        }

        public async Task AddProductsToCacheAsync(IEnumerable<ProductDto> products)
        {
            var tasks = products.Select(async product =>
            {
                var productKey = $"product:{product.Id}";

                var entries = mapper.Map<Dictionary<string, string>>(product);
                await cacheRepository.AddEntityToHashAsync(productKey, entries);
            });

            await Task.WhenAll(tasks);
        }

        public async Task AddProductsToIndexAsync(string index, IEnumerable<ProductDto> products)
        {
            var tasks = products.Select(async product =>
            {
                var productKey = $"product:{product.Id}";
                await cacheRepository.AddValueToSetAsync(index, productKey);
            });

            await Task.WhenAll(tasks);
        }

        public async Task AddTotalProductsCountToCacheAsync(string totalKey, int totalCount)
        {
            await cacheRepository.AddStringAsync(totalKey, totalCount.ToString());
        }
    }
}
