using Marten;
using ShoppingCart.API.Models;
using ShoppingCart.API.Repository;

public class ShoppingCartRepository : IShoppingCartRepository
{
    private readonly IDocumentSession session;

    public ShoppingCartRepository(IDocumentSession session)
    {
        this.session = session;
    }

    public async Task<Cart?> GetCartByUserIdAsync(string userId, CancellationToken cancellationToken)
    {
        return await session
            .Query<Cart>()
            .Where(c => c.UserId == userId)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<Cart?> GetCartByCartIdAsync(string cartId, CancellationToken cancellationToken)
    {
        return await session
            .Query<Cart>()
            .Where(c => c.CartId == cartId)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task StoreCartAsync(Cart cart, CancellationToken cancellationToken)
    {
        session.Store(cart);
        await session.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteCartAsync(Guid shoppingCartId, CancellationToken cancellationToken)
    {
        session.Delete<Cart>(shoppingCartId);
        await session.SaveChangesAsync(cancellationToken);
    }
}
