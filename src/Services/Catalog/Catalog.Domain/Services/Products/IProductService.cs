using Catalog.Domain.Models;
using Catalog.Domain.Models.Dtos;
using Catalog.Domain.Models.Pagination;

namespace Catalog.Domain.Services.Products
{
    public interface IProductService
    {
        Task<PaginatedList<ProductDto>> GetProductsByCategoryAsync(string category, PaginationParameters paginationParameters, CancellationToken cancellationToken);
        Task<PaginatedList<ProductDto>> GetProductsBySubcategoryAsync(string subcategory, PaginationParameters paginationParameters, CancellationToken cancellationToken);
        Task<PaginatedList<ProductDto>> GetProductsByNameAsync(string productName, PaginationParameters paginationParameters, CancellationToken cancellationToken);
        Task<PaginatedList<ProductDto>> GetAllProductsAsync(PaginationParameters paginationParameters, CancellationToken cancellationToken);
    }
}
