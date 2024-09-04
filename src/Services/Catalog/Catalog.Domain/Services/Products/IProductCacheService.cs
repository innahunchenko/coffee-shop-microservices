using Catalog.Domain.Models.Dtos;
using Catalog.Domain.Models.Pagination;
using Catalog.Domain.Models;

namespace Catalog.Domain.Services.Products
{
    public interface IProductCacheService
    {
        Task<PaginatedList<ProductDto>> GetProductsFromCacheAsync(string index, string totalKey, PaginationParameters paginationParameters);
        Task AddProductsToCacheAsync(IEnumerable<ProductDto> products);
        Task AddProductsToIndexAsync(string index, IEnumerable<ProductDto> products);
        Task AddTotalProductsCountToCacheAsync(string totalKey, int totalCount);
    }
}
