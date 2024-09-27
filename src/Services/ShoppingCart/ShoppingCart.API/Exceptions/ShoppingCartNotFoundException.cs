using Foundation.Exceptions;

namespace ShoppingCart.API.Exceptions
{
    public class ShoppingCartNotFoundException : NotFoundException
    {
        public ShoppingCartNotFoundException(string id) 
            : base("ShoppingCart", id) { }
    }
}
