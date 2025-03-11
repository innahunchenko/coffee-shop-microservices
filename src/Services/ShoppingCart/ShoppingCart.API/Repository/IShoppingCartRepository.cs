using ShoppingCart.API.Models;

namespace ShoppingCart.API.Repository
{
    public interface IShoppingCartRepository
    {
        Cart? GetCartByUserId(string userId);
        Cart? GetCartByCartId(string cartId);
        void StoreCart(Cart cart);
        void DeleteCart(Guid shoppingCartId);
    }
}
