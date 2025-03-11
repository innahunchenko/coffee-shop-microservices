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

    public Cart? GetCartByUserId(string userId)
    {
        return session
            .Query<Cart>()
            .Where(c => c.UserId == userId)
            .FirstOrDefault();
    }

    public Cart? GetCartByCartId(string cartId)
    {
        return session
            .Query<Cart>()
            .Where(c => c.CartId == cartId)
            .FirstOrDefault();
    }

    public void StoreCart(Cart cart)
    {
        session.Store(cart);
        session.SaveChanges();
    }

    public void DeleteCart(Guid shoppingCartId)
    {
        session.Delete<Cart>(shoppingCartId);
        session.SaveChanges();
    }
}
