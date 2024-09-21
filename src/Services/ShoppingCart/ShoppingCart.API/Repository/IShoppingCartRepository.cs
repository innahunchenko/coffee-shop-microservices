using ShoppingCart.API.Models;

namespace ShoppingCart.API.Repository
{
    public interface IShoppingCartRepository
    {
        Task<Cart> GetByUserIdAsync(string userId, CancellationToken cancellationToken);
        Task<Cart> GetBySessionIdAsync(string sessionId, CancellationToken cancellationToken);
        Task<Cart> StoreAsync(Cart cart, CancellationToken cancellationToken);
        Task<bool> DeleteAllAsync(string shoppingCartId, CancellationToken cancellationToken);
        Task<bool> DeleteProductsAsync(Cart cart, IList<ProductSelection> products, CancellationToken cancellationToken);
    }
}
