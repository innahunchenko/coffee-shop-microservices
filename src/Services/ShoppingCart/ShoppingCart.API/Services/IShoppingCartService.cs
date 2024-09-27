using ShoppingCart.API.Models;

namespace ShoppingCart.API.Services
{
    public interface IShoppingCartService
    {
        Task<Cart> GetOrCreateCartAsync(string? userId, CancellationToken cancellationToken);
        Task<Cart> StoreCartAsync(IList<ProductSelection> productSelections, CancellationToken cancellationToken);
        Task<bool> DeleteAllFromCartAsync(string? userId, CancellationToken cancellationToken);
       // Task MergeGuestCartWithUserCartAsync(string guestSessionId, string userId, CancellationToken cancellationToken);
        Task<bool> DeleteProductsAsync(string? userId, IList<ProductSelection> products, CancellationToken cancellationToken);
    }
}
