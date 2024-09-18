using Catalog.Domain.Models;
using Catalog.Domain.Models.Dtos;
using Catalog.Domain.Models.Pagination;

namespace Catalog.Domain.Services.Products
{
    public interface IProductService
    {
        Task<PaginatedList<ProductDto>> GetByCategoryAsync(string category, PaginationParameters paginationParameters);
        Task<PaginatedList<ProductDto>> GetBySubcategoryAsync(string subcategory, PaginationParameters paginationParameters);
        Task<PaginatedList<ProductDto>> GetByNameAsync(string productName, PaginationParameters paginationParameters);
        Task<PaginatedList<ProductDto>> GetAllAsync(PaginationParameters paginationParameters);
    }
}
