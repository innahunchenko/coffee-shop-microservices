using RedisCachingService;
using Catalog.Domain.Services.Products;
using Catalog.Domain.Models.Dtos;
using Catalog.Application.Mapping;
using Microsoft.Extensions.Logging;

namespace Catalog.Infrastructure.Services.Products
{
    public class ProductCacheService : IProductCacheService
    {
        private readonly IRedisCacheRepository cacheRepository;
        private readonly ILogger<ProductCacheService> logger;

        public ProductCacheService(IRedisCacheRepository cacheRepository, ILogger<ProductCacheService> logger)
        {
            this.cacheRepository = cacheRepository;
            this.logger = logger;
        }

        public async Task<ProductDto> GetProductByIdAsync(Guid productId)
        {
            var key = GetCacheKey(productId.ToString());
            var cachedProduct = await cacheRepository.GetEntityFromHashAsync(key);
            var products = cachedProduct.ToProductDto();

            return products;
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

            logger.LogInformation($"{cachedProducts.Count()} cached products with {index} retrieved from cache");

            return cachedProducts;
        }

        public async Task AddProductsToCacheAsync(IEnumerable<ProductDto> productsDto)
        {
            var tasks = productsDto.Select(async product =>
            {
                var productKey = GetCacheKey(product.Id!);

                var entity = product.ToEntity();
                var result = await cacheRepository.AddEntityToHashAsync(productKey, entity);
                logger.LogInformation($"Product {product.Id} {(result ? "added" : "has not been added")} for key {productKey} to cache");
            });
            await Task.WhenAll(tasks);
        }

        public async Task AddProductsToIndexAsync(string index, IEnumerable<ProductDto> products)
        {
            var tasks = products.Select(async product =>
            {
                var productKey = GetCacheKey(product.Id!);
                var result = await cacheRepository.AddValueToSetAsync(index, productKey);
                logger.LogInformation($"Product {product.Id} {(result ? "added" : "has not been added")} to index {index} to cache");
            });

            await Task.WhenAll(tasks);
        }

        public async Task<int> GetCachedTotalProductsCountAsync(string totalKey)
        {
            var cachedTotalProducts = await cacheRepository.GetStringAsync(totalKey);
            return string.IsNullOrEmpty(cachedTotalProducts) ? 0 : int.Parse(cachedTotalProducts);
        }

        public async Task AddTotalProductsCountToCacheAsync(string totalKey, int totalCount)
        {
            var result = await cacheRepository.AddStringAsync(totalKey, totalCount.ToString());
            logger.LogInformation($"Total count of products {totalCount} with {totalKey} {(result ? "added" : "has not been added")} to cache");
        }

        private string GetCacheKey(string productId)
        {
            return $"product:{productId.ToLower()}";
        }
    }
}
