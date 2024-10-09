using Marten;
using ShoppingCart.API.Exceptions;
using ShoppingCart.API.Models;

namespace ShoppingCart.API.Repository
{
    public class ShoppingCartRepository : IShoppingCartRepository
    {
        private readonly IDocumentStore documentStore;

        public ShoppingCartRepository(IDocumentStore documentStore)
        {
            this.documentStore = documentStore;
        }

        public async Task<Cart> GetCartByUserIdAsync(string userId, CancellationToken cancellationToken)
        {
            using var session = await documentStore.LightweightSerializableSessionAsync();
            var cart = await session
                .Query<Cart>()
                .Where(c => c.UserId == userId)
                .FirstOrDefaultAsync(cancellationToken);

            return cart is null ? throw new ShoppingCartNotFoundException(userId) : cart;
        }

        public async Task<Cart> GetCartByCartIdAsync(string cartId, CancellationToken cancellationToken)
        {
            using var session = await documentStore.LightweightSerializableSessionAsync();
            var cart = await session
                .Query<Cart>()
                .Where(c => c.CartId == cartId)
                .FirstOrDefaultAsync(cancellationToken);

            return cart is null ? throw new ShoppingCartNotFoundException(cartId) : cart;
        }

        public async Task<Cart> StoreCartAsync(Cart cart, CancellationToken cancellationToken)
        {
            try
            {
                using var session = await documentStore.LightweightSerializableSessionAsync();
                session.Store(cart);
                await session.SaveChangesAsync(cancellationToken);
            }
            catch(Exception ex) 
            { 
                Console.WriteLine(ex.ToString());
            }
            return cart;
        }

        public async Task<bool> DeleteAllFromCartAsync(Guid shoppingCartId, CancellationToken cancellationToken)
        {
            using var session = await documentStore.LightweightSerializableSessionAsync();
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

            using var session = await documentStore.LightweightSerializableSessionAsync();
            session.Store(cart);
            await session.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}
