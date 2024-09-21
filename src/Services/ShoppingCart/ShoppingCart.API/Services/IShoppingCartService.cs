using ShoppingCart.API.Models;

namespace ShoppingCart.API.Services
{
    public interface IShoppingCartService
    {
        Task<Cart> GetAsync(string? userId, CancellationToken cancellationToken);
        Task<Cart> StoreAsync(Cart cart, CancellationToken cancellationToken);
        Task<bool> DeleteAllProductsAsync(string shoppingCartId, CancellationToken cancellationToken);
        Task MergeGuestCartWithUserCartAsync(string guestSessionId, string userId, CancellationToken cancellationToken);
        Task<bool> DeleteProductsAsync(string userId, IList<ProductSelection> products, CancellationToken cancellationToken);
    }
}
