using Catalog.Domain.Models;
using Catalog.Domain.Models.Dtos;
using Catalog.Domain.Models.Pagination;

namespace Catalog.Domain.Services.Products
{
    public interface IProductService
    {
        Task<PaginatedList<ProductDto>> GetProductsByCategoryAsync(string category, PaginationParameters paginationParameters);
        Task<PaginatedList<ProductDto>> GetProductsBySubcategoryAsync(string subcategory, PaginationParameters paginationParameters);
        Task<PaginatedList<ProductDto>> GetProductsByNameAsync(string productName, PaginationParameters paginationParameters);
        Task<PaginatedList<ProductDto>> GetAllProductsAsync(PaginationParameters paginationParameters);
        Task<ProductDto> GetProductByIdAsync(Guid id);
        Task<IEnumerable<ProductDto>> GetProductsByIdsAsync(IEnumerable<Guid> productIds);
    }
}
