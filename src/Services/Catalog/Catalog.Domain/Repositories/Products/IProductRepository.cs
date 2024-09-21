using Catalog.Domain.Models;
using Catalog.Domain.Models.Dtos;
using Catalog.Domain.Models.Pagination;


namespace Catalog.Domain.Repositories.Products
{
    public interface IProductRepository
    {
        Task<IEnumerable<ProductDto>> GetProductsByCategoryAsync(string category, PaginationParameters paginationParameters);
        Task<IEnumerable<ProductDto>> GetProductsBySubcategoryAsync(string subcategory, PaginationParameters paginationParameters);
        Task<IEnumerable<ProductDto>> GetProductsByNameAsync(string productName, PaginationParameters paginationParameters);
        Task<IEnumerable<ProductDto>> GetAllProductsAsync(PaginationParameters paginationParameters);
        Task<ProductDto?> GetProductByIdAsync(Guid productId);
        Task<int> GetAllProductsTotalCountAsync();
        Task<int> GetProductsTotalCountByCategoryAsync(string category);
        Task<int> GetProductsTotalCountBySubcategoryAsync(string subcategory);
        Task<int> GetProductsTotalCountByNameAsync(string productName);
    }
}
