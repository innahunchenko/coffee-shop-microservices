using Foundation.Exceptions;

namespace ShoppingCart.API.Exceptions
{
    public class ShoppingCartNotFoundException : NotFoundException
    {
        public ShoppingCartNotFoundException(string shoppingCartId) 
            : base("ShoppingCart", shoppingCartId) { }
    }
}
