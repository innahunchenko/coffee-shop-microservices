using Marten;
using ShoppingCart.API.Exceptions;
using ShoppingCart.API.Models;

namespace ShoppingCart.API.Repository
{
    public class ShoppingCartRepository(IDocumentSession session) : IShoppingCartRepository
    {
        public async Task<Cart> GetCartByUserIdAsync(string userId, CancellationToken cancellationToken)
        {
            var cart = await session
                .Query<Cart>()
                .Where(c => c.UserId == userId)
                .FirstOrDefaultAsync(cancellationToken);

            return cart is null ? throw new ShoppingCartNotFoundException(userId) : cart;
        }

        public async Task<Cart> GetCartByCartIdAsync(string cartId, CancellationToken cancellationToken)
        {
            var cart = await session
                .Query<Cart>()
                .Where(c => c.CartId == cartId)
                .FirstOrDefaultAsync(cancellationToken);

            return cart is null ? throw new ShoppingCartNotFoundException(cartId) : cart;
        }

        public async Task<Cart> StoreCartAsync(Cart cart, CancellationToken cancellationToken)
        {
            session.Store(cart);
            await session.SaveChangesAsync(cancellationToken);
            return cart;
        }

        public async Task<bool> DeleteAllFromCartAsync(Guid shoppingCartId, CancellationToken cancellationToken)
        {
            session.Delete<Cart>(shoppingCartId);
            await session.SaveChangesAsync(cancellationToken);
            return true;
        }

        public async Task<bool> DeleteProductsFromCartAsync(Cart cart, IList<ProductSelection> products, CancellationToken cancellationToken)
        {
            foreach (var product in products)
            {
                var productToRemove = cart.Selections.FirstOrDefault(p => p.ProductId == product.ProductId);
                if (productToRemove != null)
                {
                    cart.Selections.Remove(productToRemove);
                }
            }

            session.Store(cart);
            await session.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}
