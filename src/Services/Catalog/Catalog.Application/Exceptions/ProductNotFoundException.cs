using Foundation.Exceptions;

namespace Catalog.Application.Exceptions
{
    public class ProductNotFoundException : NotFoundException
    {
        public ProductNotFoundException(string id)
            : base("Product", id) { }
    }
}
