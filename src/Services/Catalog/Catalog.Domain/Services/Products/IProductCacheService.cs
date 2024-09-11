using Catalog.Domain.Models.Pagination;
using Catalog.Domain.Models;
using Catalog.Domain.Models.Dtos;

namespace Catalog.Domain.Services.Products
{
    public interface IProductCacheService
    {
        Task<PaginatedList<ProductDto>> GetProductsFromCacheAsync(string index, string totalKey, PaginationParameters paginationParameters);
        Task AddProductsToCacheAsync(IEnumerable<ProductDto> products, CancellationToken cancellationToken);
        Task AddProductsToIndexAsync(string index, IEnumerable<ProductDto> products, CancellationToken cancellationToken);
        Task AddTotalProductsCountToCacheAsync(string totalKey, int totalCount, CancellationToken cancellationToken);
    }
}
