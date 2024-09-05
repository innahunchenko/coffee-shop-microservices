using Catalog.Domain.Models;
using Catalog.Domain.Models.Pagination;


namespace Catalog.Domain.Repositories.Products
{
    public interface IProductRepository
    {
        Task<IEnumerable<Product>> GetProductsByCategoryAsync(string category, PaginationParameters paginationParameters, CancellationToken cancellationToken);
        Task<IEnumerable<Product>> GetProductsBySubcategoryAsync(string subcategory, PaginationParameters paginationParameters, CancellationToken cancellationToken);
        Task<IEnumerable<Product>> GetProductsByProductNameAsync(string productName, PaginationParameters paginationParameters, CancellationToken cancellationToken);
        Task<IEnumerable<Product>> GetAllProductsAsync(PaginationParameters paginationParameters, CancellationToken cancellationToken);
        Task<int> GetAllProductsTotalCountAsync();
        Task<int> GetCategoryProductsTotalCountAsync(string category);
        Task<int> GetSubcategoryProductsTotalCountAsync(string subcategory);
        Task<int> GetProductNameTotalCountAsync(string productName);
    }
}
