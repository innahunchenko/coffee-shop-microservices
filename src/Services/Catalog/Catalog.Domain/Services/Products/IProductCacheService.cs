using Catalog.Domain.Models.Dtos;

namespace Catalog.Domain.Services.Products
{
    public interface IProductCacheService
    {
        Task<IList<ProductDto>> GetProductsFromCacheAsync(string index);
        Task<int> GetCachedTotalProductsCountAsync(string totalKey);
        Task AddProductsToCacheAsync(IEnumerable<ProductDto> products);
        Task AddProductsToIndexAsync(string index, IEnumerable<ProductDto> products);
        Task AddTotalProductsCountToCacheAsync(string totalKey, int totalCount);
        Task<ProductDto> GetCahcedProductByIdAsync(Guid productId);
        Task<IEnumerable<ProductDto>> GetCachedProductsByIdsAsync(IEnumerable<Guid> productIds);
    }
}
