using Catalog.Domain.Models.Dtos;

namespace Catalog.Domain.Services.Products
{
    public interface IProductCacheService
    {
        Task<IList<ProductDto>> GetProductsFromCacheAsync(string index);
        Task<int> GetCachedTotalProductsCountAsync(string totalKey);
        Task AddProductsToCacheAsync(IEnumerable<ProductDto> products, CancellationToken cancellationToken);
        Task AddProductsToIndexAsync(string index, IEnumerable<ProductDto> products, CancellationToken cancellationToken);
        Task AddTotalProductsCountToCacheAsync(string totalKey, int totalCount, CancellationToken cancellationToken);
    }
}
