using Catalog.Domain.Models;
using Catalog.Domain.Models.Dtos;
using Catalog.Domain.Models.Pagination;
using Catalog.Domain.Services.Products;

namespace Catalog.Infrastructure.Services.Products
{
    public class ProductServiceCacheDecorator : IProductService
    {
        private readonly IProductService decorated;
        private readonly IProductCacheService cacheService;

        public static readonly string SubcategoryIndexTemplate = "index:product:subcategory:{0}";
        public static readonly string CategoryIndexTemplate = "index:product:category:{0}";
        public static readonly string ProductNameIndexTemplate = "index:product:name:{0}";
        public static readonly string AllProductsIndexTemplate = "index:product:all";

        public ProductServiceCacheDecorator(IProductService decorated, IProductCacheService cacheService)
        {
            this.decorated = decorated;
            this.cacheService = cacheService;
        }

        public async Task<PaginatedList<ProductDto>> GetProductsBySubcategoryAsync(string subcategory, PaginationParameters paginationParameters)
        {
            var cachedContext = new CachedContext(SubcategoryIndexTemplate, subcategory, paginationParameters);
            return await GetProductsAsync(
                () => decorated.GetProductsBySubcategoryAsync(subcategory, paginationParameters), cachedContext);
        }

        public async Task<PaginatedList<ProductDto>> GetProductsByCategoryAsync(string category, PaginationParameters paginationParameters)
        {
            var cachedContext = new CachedContext(CategoryIndexTemplate, category, paginationParameters);
            return await GetProductsAsync(
                () => decorated.GetProductsByCategoryAsync(category, paginationParameters), cachedContext);
        }

        public async Task<PaginatedList<ProductDto>> GetProductsByNameAsync(string name, PaginationParameters paginationParameters)
        {
            var cachedContext = new CachedContext(ProductNameIndexTemplate, name, paginationParameters);
            return await GetProductsAsync(
                () => decorated.GetProductsByNameAsync(name, paginationParameters), cachedContext);
        }

        public async Task<PaginatedList<ProductDto>> GetAllProductsAsync(PaginationParameters paginationParameters)
        {
            var cachedContext = new CachedContext(AllProductsIndexTemplate, string.Empty, paginationParameters);
            return await GetProductsAsync(
                () => decorated.GetAllProductsAsync(paginationParameters), cachedContext);
        }

        private async Task<PaginatedList<ProductDto>> GetProductsAsync(Func<Task<PaginatedList<ProductDto>>> getFromDb, CachedContext cachedContext)
        {
            var cachedProducts  = await cacheService.GetProductsFromCacheAsync(cachedContext.Index);
            var cachedTotalCount = await cacheService.GetCachedTotalProductsCountAsync(cachedContext.TotalKey);
            
            if (cachedProducts.Count() != 0)
            {
                return new PaginatedList<ProductDto>(cachedProducts, cachedTotalCount, cachedContext.PaginationParameters.PageSize);
            }

            var dbProducts = await getFromDb();

            if (dbProducts.Items.Count() == 0)
            {
                return new PaginatedList<ProductDto>(new List<ProductDto>(), 0, cachedContext.PaginationParameters.PageSize);
            }

            await cacheService.AddProductsToCacheAsync(dbProducts.Items);
            await cacheService.AddProductsToIndexAsync(cachedContext.Index, dbProducts.Items);
            await cacheService.AddTotalProductsCountToCacheAsync(cachedContext.TotalKey, dbProducts.TotalCount);

            return dbProducts;
        }

        public async Task<ProductDto> GetProductByIdAsync(Guid productId)
        {
            var cachedProduct = await cacheService.GetCahcedProductByIdAsync(productId);
            
            if (cachedProduct != null && !string.IsNullOrEmpty(cachedProduct.Id)) 
            {
                return cachedProduct;
            }

            var dbProduct = await decorated.GetProductByIdAsync(productId);
            await cacheService.AddProductsToCacheAsync(new List<ProductDto> { dbProduct });

            return dbProduct;
        }

        public async Task<IEnumerable<ProductDto>> GetProductsByIdsAsync(IEnumerable<Guid> productIds)
        {
            var cachedProducts = await cacheService.GetCachedProductsByIdsAsync(productIds);

            if (cachedProducts != null && cachedProducts.All(product => !string.IsNullOrEmpty(product.Id)))
            {
                return cachedProducts;
            }

            var dbProducts = await decorated.GetProductsByIdsAsync(productIds);
            await cacheService.AddProductsToCacheAsync(dbProducts);

            return dbProducts;
        }
    }
}
