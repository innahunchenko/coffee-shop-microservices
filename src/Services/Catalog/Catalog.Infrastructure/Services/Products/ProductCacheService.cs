using RedisCachingService;
using Catalog.Domain.Services.Products;
using Catalog.Domain.Models.Dtos;
using Catalog.Application.Mapping;

namespace Catalog.Infrastructure.Services.Products
{
    public class ProductCacheService : IProductCacheService
    {
        private readonly IRedisCacheRepository cacheRepository;

        public ProductCacheService(IRedisCacheRepository cacheRepository)
        {
            this.cacheRepository = cacheRepository;
        }

        public async Task<IList<ProductDto>> GetProductsFromCacheAsync(string index)
        {
            var productsKeys = await cacheRepository.GetValuesFromSetAsync(index);

            var cachedProducts = new List<ProductDto>();

            foreach (var key in productsKeys)
            {
                var entryDictionary = await cacheRepository.GetEntityFromHashAsync(key);
                if (entryDictionary != null && entryDictionary.Count > 0)
                {
                    var productDto = entryDictionary.ToProductDto();
                    cachedProducts.Add(productDto);
                }
            }
            
            return cachedProducts;
        }

        public async Task AddProductsToCacheAsync(IEnumerable<ProductDto> productsDto, CancellationToken cancellationToken)
        {
            var tasks = productsDto.Select(async product =>
            {
                var productKey = GetCacheKey(product.Id);

                var entity = product.ToEntity();
                await cacheRepository.AddEntityToHashAsync(productKey, entity, cancellationToken);
            });

            await Task.WhenAll(tasks);
        }

        public async Task AddProductsToIndexAsync(string index, IEnumerable<ProductDto> products, CancellationToken cancellationToken)
        {
            var tasks = products.Select(async product =>
            {
                var productKey = GetCacheKey(product.Id);
                await cacheRepository.AddValueToSetAsync(index, productKey, cancellationToken);
            });

            await Task.WhenAll(tasks);
        }

        public async Task<int> GetCachedTotalProductsCountAsync(string totalKey)
        {
            var cachedTotalProducts = await cacheRepository.GetStringAsync(totalKey);
            return string.IsNullOrEmpty(cachedTotalProducts) ? 0 : int.Parse(cachedTotalProducts);
        }

        public async Task AddTotalProductsCountToCacheAsync(string totalKey, int totalCount, CancellationToken cancellationToken)
        {
            await cacheRepository.AddStringAsync(totalKey, totalCount.ToString(), cancellationToken);
        }

        private string GetCacheKey(string productId)
        {
            return $"product:{productId}";
        }
    }
}
