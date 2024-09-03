using Catalog.Domain.Models;
using Catalog.Domain.Models.Pagination;


namespace Catalog.Domain.Repositories.Products
{
    public interface IProductRepository
    {
        Task<IEnumerable<Product>> GetProductsByCategoryAsync(string category, PaginationParameters paginationParameters);
        Task<IEnumerable<Product>> GetProductsBySubcategoryAsync(string subcategory, PaginationParameters paginationParameters);
        Task<IEnumerable<Product>> GetProductsByProductNameAsync(string productName, PaginationParameters paginationParameters);
        Task<IEnumerable<Product>> GetAllProductsAsync(PaginationParameters paginationParameters);
        Task<int> GetAllProductsTotalCountAsync();
        Task<int> GetCategoryProductsTotalCountAsync(string category);
        Task<int> GetSubcategoryProductsTotalCountAsync(string subcategory);
        Task<int> GetProductNameTotalCountAsync(string productName);
    }
}
