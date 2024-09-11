using Catalog.Domain.Models.Pagination;
using Catalog.Domain.Models;
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

        public async Task<PaginatedList<ProductDto>> GetProductsFromCacheAsync(string index, string totalKey, PaginationParameters paginationParameters)
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

            if (!cachedProducts.Any())
            {
                return new PaginatedList<ProductDto>(new List<ProductDto>(), 0, paginationParameters.PageSize);
            }

            var cachedTotalProducts = await cacheRepository.GetStringAsync(totalKey);
            int totalProducts = string.IsNullOrEmpty(cachedTotalProducts) ? 0 : int.Parse(cachedTotalProducts);

            return new PaginatedList<ProductDto>(cachedProducts, totalProducts, paginationParameters.PageSize);
        }

        public async Task AddProductsToCacheAsync(IEnumerable<ProductDto> productsDto, CancellationToken cancellationToken)
        {
            var tasks = productsDto.Select(async product =>
            {
                var productKey = $"product:{product.Id}";

                var entity = product.ToEntity();
                await cacheRepository.AddEntityToHashAsync(productKey, entity, cancellationToken);
            });

            await Task.WhenAll(tasks);
        }

        public async Task AddProductsToIndexAsync(string index, IEnumerable<ProductDto> products, CancellationToken cancellationToken)
        {
            var tasks = products.Select(async product =>
            {
                var productKey = $"product:{product.Id}";
                await cacheRepository.AddValueToSetAsync(index, productKey, cancellationToken);
            });

            await Task.WhenAll(tasks);
        }

        public async Task AddTotalProductsCountToCacheAsync(string totalKey, int totalCount, CancellationToken cancellationToken)
        {
            await cacheRepository.AddStringAsync(totalKey, totalCount.ToString(), cancellationToken);
        }
    }
}
