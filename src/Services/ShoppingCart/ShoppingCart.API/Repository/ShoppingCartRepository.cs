using Marten;
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

        public async Task<Cart?> GetCartByUserIdAsync(string userId, CancellationToken cancellationToken)
        {
            using var session = await documentStore.LightweightSerializableSessionAsync();
            return await session
                .Query<Cart>()
                .Where(c => c.UserId == userId)
                .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<Cart?> GetCartByCartIdAsync(string cartId, CancellationToken cancellationToken)
        {
            using var session = await documentStore.LightweightSerializableSessionAsync();
            return await session
                .Query<Cart>()
                .Where(c => c.CartId == cartId)
                .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task StoreCartAsync(Cart cart, CancellationToken cancellationToken)
        {
            using var session = await documentStore.LightweightSerializableSessionAsync();
            session.Store(cart);
            await session.SaveChangesAsync(cancellationToken);
        }

        public async Task DeleteAllFromCartAsync(Guid shoppingCartId, CancellationToken cancellationToken)
        {
            using var session = await documentStore.LightweightSerializableSessionAsync();
            session.Delete<Cart>(shoppingCartId);
            await session.SaveChangesAsync(cancellationToken);
        }

        public async Task DeleteProductsFromCartAsync(Cart cart, IList<ProductSelection> products, CancellationToken cancellationToken)
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
        }
    }
}
