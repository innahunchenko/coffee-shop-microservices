using Marten;
using ShoppingCart.API.Models;

namespace ShoppingCart.API.Repository
{
    public class CartConfiguration : MartenRegistry
    {
        public CartConfiguration()
        {
            For<Cart>()
            .Identity(x => x.Id) 
            .Index(x => x.CartId)
            .Index(x => x.UserId);
        }
    }
}
