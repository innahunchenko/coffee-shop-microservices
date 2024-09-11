using Catalog.Domain.Models;
using Catalog.Domain.Models.Dtos;
using Catalog.Domain.Models.Pagination;
using Catalog.Domain.Services.Products;

namespace Catalog.Infrastructure.Services.Products
{
    public class ProductServiceCacheDecorator : IProductService
    {
        private readonly IProductService productService;
        private readonly IProductCacheService cacheService;

        public static readonly string SubcategoryIndexTemplate = "index:product:subcategory:{0}";
        public static readonly string CategoryIndexTemplate = "index:product:category:{0}";
        public static readonly string ProductNameIndexTemplate = "index:product:name:{0}";
        public static readonly string AllProductsIndexTemplate = "index:product:all";

        public ProductServiceCacheDecorator(IProductService productService, IProductCacheService cacheService)
        {
            this.productService = productService;
            this.cacheService = cacheService;
        }

        public async Task<PaginatedList<ProductDto>> GetProductsBySubcategoryAsync(string subcategory, 
            PaginationParameters paginationParameters, CancellationToken cancellationToken)
        {
            return await GetProductsAsync(
                () => productService.GetProductsBySubcategoryAsync(subcategory, paginationParameters, cancellationToken),
                SubcategoryIndexTemplate,
                subcategory,
                paginationParameters,
                cancellationToken
            );
        }

        public async Task<PaginatedList<ProductDto>> GetProductsByCategoryAsync(string category, 
            PaginationParameters paginationParameters, CancellationToken cancellationToken)
        {
            return await GetProductsAsync(
                () => productService.GetProductsByCategoryAsync(category, paginationParameters, cancellationToken),
                CategoryIndexTemplate,
                category,
                paginationParameters,
                cancellationToken
            );
        }

        public async Task<PaginatedList<ProductDto>> GetProductsByNameAsync(string name, 
            PaginationParameters paginationParameters, CancellationToken cancellationToken)
        {
            return await GetProductsAsync(
                () => productService.GetProductsByNameAsync(name, paginationParameters, cancellationToken),
                ProductNameIndexTemplate,
                name,
                paginationParameters,
                cancellationToken
            );
        }

        public async Task<PaginatedList<ProductDto>> GetAllProductsAsync(PaginationParameters paginationParameters, 
            CancellationToken cancellationToken)
        {
            return await GetProductsAsync(
                () => productService.GetAllProductsAsync(paginationParameters, cancellationToken),
                AllProductsIndexTemplate,
                string.Empty,
                paginationParameters, 
                cancellationToken
            );
        }

        private async Task<PaginatedList<ProductDto>> GetProductsAsync(
            Func<Task<PaginatedList<ProductDto>>> getProductsFromDbFunc,
            string indexKeyTemplate, string filterKey, PaginationParameters paginationParameters, 
            CancellationToken cancellationToken)
        {
            var index = string.Format(indexKeyTemplate + ":page:{1}", filterKey.ToLower(), paginationParameters.PageNumber);
            var totalKey = $"{index}:total";

            var cachedResult = await cacheService.GetProductsFromCacheAsync(index, totalKey, paginationParameters);
            if (cachedResult.Items.Count() != 0)
            {
                Console.WriteLine($"Products {index}, {cachedResult.TotalCount} from cache");
                return cachedResult;
            }

            var productsFromDb = await getProductsFromDbFunc();
            if (productsFromDb.Items.Count() == 0)
            {
                return new PaginatedList<ProductDto>(new List<ProductDto>(), 0, paginationParameters.PageSize);
            }

            await cacheService.AddProductsToCacheAsync(productsFromDb.Items, cancellationToken);
            await cacheService.AddProductsToIndexAsync(index, productsFromDb.Items, cancellationToken);
            await cacheService.AddTotalProductsCountToCacheAsync(totalKey, productsFromDb.TotalCount, cancellationToken);

            return productsFromDb;
        }
    }
}
