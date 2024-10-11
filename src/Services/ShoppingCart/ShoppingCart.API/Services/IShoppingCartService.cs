using ShoppingCart.API.Models;

namespace ShoppingCart.API.Services
{
    public interface IShoppingCartService
    {
        Task<Cart> GetOrCreateCartAsync(string? userId, CancellationToken cancellationToken);
        Task<Cart> StoreCartAsync(IList<ProductSelection> productSelections, CancellationToken cancellationToken);
        Task DeleteAllProductsFromCartAsync(string? userId, CancellationToken cancellationToken);
        Task DeleteProductsFromCartAsync(string? userId, IList<ProductSelection> products, CancellationToken cancellationToken);
    }
}
