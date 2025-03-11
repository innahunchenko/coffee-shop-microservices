using ShoppingCart.API.Models;

namespace ShoppingCart.API.Services
{
    public interface IShoppingCartService
    {
        Cart GetOrCreateCart();
        Cart StoreCart(IList<ProductSelection> productSelections);
        void DeleteCart(Guid shoppingCartId);
    }
}
