using ShoppingCart.API.Models;

namespace ShoppingCart.API.Services
{
    public interface IShoppingCartService
    {
        Task<Cart> GetOrCreateCartAsync(CancellationToken cancellationToken);
        Task<Cart> StoreCartAsync(IList<ProductSelection> productSelections, CancellationToken cancellationToken);
        Task DeleteCartAsync(Guid shoppingCartId, CancellationToken cancellationToken);
    }
}
