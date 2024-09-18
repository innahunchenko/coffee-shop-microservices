using Catalog.Domain.Models;
using Catalog.Domain.Models.Dtos;
using Catalog.Domain.Models.Pagination;


namespace Catalog.Domain.Repositories.Products
{
    public interface IProductRepository
    {
        Task<IEnumerable<ProductDto>> GetByCategoryAsync(string category, PaginationParameters paginationParameters);
        Task<IEnumerable<ProductDto>> GetBySubcategoryAsync(string subcategory, PaginationParameters paginationParameters);
        Task<IEnumerable<ProductDto>> GetByProductNameAsync(string productName, PaginationParameters paginationParameters);
        Task<IEnumerable<ProductDto>> GetAllAsync(PaginationParameters paginationParameters);
        Task<int> GetAllTotalCountAsync();
        Task<int> GetTotalCountByCategoryAsync(string category);
        Task<int> GetTotalCountBySubcategoryAsync(string subcategory);
        Task<int> GetTotalCountByNameAsync(string productName);
    }
}
