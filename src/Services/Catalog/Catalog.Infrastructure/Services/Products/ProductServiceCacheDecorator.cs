using Catalog.Domain.Models;
using Catalog.Domain.Models.Dtos;
using Catalog.Domain.Models.Pagination;
using Catalog.Domain.Services.Products;
using Microsoft.Extensions.Logging;

namespace Catalog.Infrastructure.Services.Products
{
    public class ProductServiceCacheDecorator : IProductService
    {
        private readonly IProductService productService;
        private readonly IProductCacheService cacheService;
        private readonly ILogger<ProductServiceCacheDecorator> logger;

        public static readonly string SubcategoryIndexTemplate = "index:product:subcategory:{0}";
        public static readonly string CategoryIndexTemplate = "index:product:category:{0}";
        public static readonly string ProductNameIndexTemplate = "index:product:name:{0}";
        public static readonly string AllProductsIndexTemplate = "index:product:all";

        public ProductServiceCacheDecorator(IProductService productService, 
            IProductCacheService cacheService, ILogger<ProductServiceCacheDecorator> logger)
        {
            this.productService = productService;
            this.cacheService = cacheService;
            this.logger = logger;
        }

        public async Task<PaginatedList<ProductDto>> GetProductsBySubcategoryAsync(string subcategory, 
            PaginationParameters paginationParameters, CancellationToken ct)
        {
            var cachedContext = new CachedContext(SubcategoryIndexTemplate, subcategory, paginationParameters);
            return await GetProductsAsync(
                () => productService.GetProductsBySubcategoryAsync(subcategory, paginationParameters, ct), cachedContext, ct);
        }

        public async Task<PaginatedList<ProductDto>> GetProductsByCategoryAsync(string category, 
            PaginationParameters paginationParameters, CancellationToken ct)
        {
            var cachedContext = new CachedContext(CategoryIndexTemplate, category, paginationParameters);
            return await GetProductsAsync(
                () => productService.GetProductsByCategoryAsync(category, paginationParameters, ct), cachedContext, ct);
        }

        public async Task<PaginatedList<ProductDto>> GetProductsByNameAsync(string name, 
            PaginationParameters paginationParameters, CancellationToken ct)
        {
            var cachedContext = new CachedContext(ProductNameIndexTemplate, name, paginationParameters);
            return await GetProductsAsync(
                () => productService.GetProductsByNameAsync(name, paginationParameters, ct), cachedContext, ct);
        }

        public async Task<PaginatedList<ProductDto>> GetAllProductsAsync(PaginationParameters paginationParameters, 
            CancellationToken ct)
        {
            var cachedContext = new CachedContext(AllProductsIndexTemplate, string.Empty, paginationParameters);
            return await GetProductsAsync(
                () => productService.GetAllProductsAsync(paginationParameters, ct), cachedContext, ct);
        }

        private async Task<PaginatedList<ProductDto>> GetProductsAsync(
            Func<Task<PaginatedList<ProductDto>>> getProductsFromDbFunc, CachedContext cachedContext, CancellationToken ct)
        {
            var cachedProducts  = await cacheService.GetProductsFromCacheAsync(cachedContext.Index);
            var cachedTotalCount = await cacheService.GetCachedTotalProductsCountAsync(cachedContext.TotalKey);
            
            if (cachedProducts.Count() != 0)
            {
                logger.LogInformation($"{cachedTotalCount} products with {cachedContext.Index} retrieved from cache");
                return new PaginatedList<ProductDto>(cachedProducts, cachedTotalCount, cachedContext.PaginationParameters.PageSize);
            }

            var productsFromDb = await getProductsFromDbFunc();

            if (productsFromDb.Items.Count() == 0)
            {
                return new PaginatedList<ProductDto>(new List<ProductDto>(), 0, cachedContext.PaginationParameters.PageSize);
            }

            await cacheService.AddProductsToCacheAsync(productsFromDb.Items, ct);
            logger.LogInformation($"{productsFromDb.Items.Count()} products added to cache");

            await cacheService.AddProductsToIndexAsync(cachedContext.Index, productsFromDb.Items, ct);
            logger.LogInformation($"Products added to index {cachedContext.Index}");
            
            await cacheService.AddTotalProductsCountToCacheAsync(cachedContext.TotalKey, productsFromDb.TotalCount, ct);
            logger.LogInformation($"Total products count {productsFromDb.TotalCount} added to cache by key {cachedContext.TotalKey}");

            return productsFromDb;
        }
    }

    public class CachedContext
    {
        public string Index { get; set; }
        public string TotalKey { get; set; }
        public PaginationParameters PaginationParameters { get; set; }

        public CachedContext(string indexKeyTemplate, string filterKey, PaginationParameters paginationParameters)
        {
            Index = string.Format(indexKeyTemplate + ":page:{1}", filterKey.ToLower(), paginationParameters.PageNumber);
            TotalKey = $"{Index}:total";
            PaginationParameters = paginationParameters;
        }
    }
}
