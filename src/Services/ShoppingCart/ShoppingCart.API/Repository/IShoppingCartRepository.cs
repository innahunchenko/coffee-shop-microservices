using ShoppingCart.API.Models;

namespace ShoppingCart.API.Repository
{
    public interface IShoppingCartRepository
    {
        Task<Cart?> GetCartByUserIdAsync(string userId, CancellationToken cancellationToken);
        Task<Cart?> GetCartByCartIdAsync(string cartId, CancellationToken cancellationToken);
        Task StoreCartAsync(Cart cart, CancellationToken cancellationToken);
        Task DeleteAllFromCartAsync(Guid shoppingCartId, CancellationToken cancellationToken);
        Task DeleteProductsFromCartAsync(Cart cart, IList<ProductSelection> products, CancellationToken cancellationToken);
    }
}
