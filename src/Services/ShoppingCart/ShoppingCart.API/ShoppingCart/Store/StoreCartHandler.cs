using MediatR;
using ShoppingCart.API.Models;
using ShoppingCart.API.Services;

namespace ShoppingCart.API.ShoppingCart.Store
{
    public record StoreCartRequest(List<ProductSelection> Selections) : IRequest<Cart>;
    public sealed class StoreCartHandler(IShoppingCartService service) : IRequestHandler<StoreCartRequest, Cart>
    {
        public Task<Cart> Handle(StoreCartRequest request, CancellationToken cancellationToken)
        {
            return service.StoreCartAsync(new Cart() { Selections = request.Selections }, cancellationToken);
        }
    }
}
