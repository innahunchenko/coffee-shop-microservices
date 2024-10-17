using MassTransit;
using MediatR;
using Messaging.Events;
using ShoppingCart.API.Dtos;
using ShoppingCart.API.Services;

namespace ShoppingCart.API.ShoppingCart
{
    public record CheckoutCartRequest(CartCheckoutDto CartCheckoutDto) : IRequest<CheckoutBasketResult>;
    public record CheckoutBasketResult(bool IsSuccess); 

    public sealed class CheckoutCartHandler(IShoppingCartService service, IPublishEndpoint publishEndpoint) : IRequestHandler<CheckoutCartRequest, CheckoutBasketResult>
    {
        public async Task<CheckoutBasketResult> Handle(CheckoutCartRequest request, CancellationToken cancellationToken)
        {
            var cart = await service.GetOrCreateCartAsync(null, cancellationToken);
            if (cart == null)
            {
                return new CheckoutBasketResult(false);
            }

            var eventMessage = new ShoppingCartCheckoutEvent()
            {
                AddressLine = request.CartCheckoutDto.AddressLine,
                CardName = request.CartCheckoutDto.CardName,
                CardNumber = request.CartCheckoutDto.CardNumber,
                Country = request.CartCheckoutDto.Country,
                CVV = request.CartCheckoutDto.CVV,
                EmailAddress = request.CartCheckoutDto.EmailAddress,
                Expiration = request.CartCheckoutDto.Expiration,
                FirstName = request.CartCheckoutDto.FirstName,
                LastName = request.CartCheckoutDto.LastName,
                State = request.CartCheckoutDto.State,
                ZipCode = request.CartCheckoutDto.ZipCode,
                TotalPrice = cart.TotalPrice,
            };

            eventMessage.ProductSelections = cart.Selections.Zip(eventMessage.ProductSelections, (selection, productSelection) =>
            {
                productSelection.ProductId = selection.ProductId;
                productSelection.Price = selection.Price;
                productSelection.Quantity = selection.Quantity;
                productSelection.ProductName = selection.ProductName;
                return productSelection;
            }).ToList();

            await publishEndpoint.Publish(eventMessage, cancellationToken);
            await service.DeleteCartAsync(cart.Id, cancellationToken);
            return new CheckoutBasketResult(true);
        }
    }
}
