using MediatR;
using ShoppingCart.API.Models;
using ShoppingCart.API.Services;

namespace ShoppingCart.API.ShoppingCart
{
    public record StoreCartRequest(IList<ProductSelection> Selections) : IRequest<Cart>;
    public sealed class StoreCartHandler(IShoppingCartService service) : IRequestHandler<StoreCartRequest, Cart>
    {
        public Task<Cart> Handle(StoreCartRequest request, CancellationToken cancellationToken)
        {
            return Task.FromResult(service.StoreCart(request.Selections));
        }
    }
}
