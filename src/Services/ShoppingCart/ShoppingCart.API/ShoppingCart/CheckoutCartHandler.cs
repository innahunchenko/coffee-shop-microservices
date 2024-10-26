using FluentValidation;
using LanguageExt.Common;
using MassTransit;
using MediatR;
using Messaging.Events;
using ShoppingCart.API.Dtos;
using ShoppingCart.API.Services;

namespace ShoppingCart.API.ShoppingCart
{
    public record CheckoutCartRequest(CartCheckoutDto CartCheckoutDto) : IRequest<Result<CheckoutBasketResult>>;
    public record CheckoutBasketResult(CartCheckoutDto CartCheckoutDto); 

    public sealed class CheckoutCartHandler(IShoppingCartService service, 
        IPublishEndpoint publishEndpoint, 
        IValidator<CheckoutCartRequest> validator) : IRequestHandler<CheckoutCartRequest, Result<CheckoutBasketResult>>
    {
        public async Task<Result<CheckoutBasketResult>> Handle(CheckoutCartRequest checkoutCartRequest, CancellationToken cancellationToken)
        {
            var cart = await service.GetOrCreateCartAsync(null, cancellationToken);

            var validationResult = await validator.ValidateAsync(checkoutCartRequest);

            if (!validationResult.IsValid)
            {
                var validationException = new ValidationException(validationResult.Errors);
                return new Result<CheckoutBasketResult>(validationException);
            }

            var eventMessage = new CartCheckoutEvent()
            {
                AddressLine = checkoutCartRequest.CartCheckoutDto.AddressLine,
                CardName = checkoutCartRequest.CartCheckoutDto.CardName,
                CardNumber = checkoutCartRequest.CartCheckoutDto.CardNumber,
                Country = checkoutCartRequest.CartCheckoutDto.Country,
                CVV = checkoutCartRequest.CartCheckoutDto.CVV,
                EmailAddress = checkoutCartRequest.CartCheckoutDto.EmailAddress,
                PhoneNumber = checkoutCartRequest.CartCheckoutDto.PhoneNumber,
                Expiration = checkoutCartRequest.CartCheckoutDto.Expiration,
                FirstName = checkoutCartRequest.CartCheckoutDto.FirstName,
                LastName = checkoutCartRequest.CartCheckoutDto.LastName,
                State = checkoutCartRequest.CartCheckoutDto.State,
                ZipCode = checkoutCartRequest.CartCheckoutDto.ZipCode
            };

            for (int i = 0; i < cart.Selections.Count; i++)
            {
                eventMessage.ProductSelections.Add(new ProductSelectionDto()
                {
                    ProductId = cart.Selections[i].ProductId,
                    ProductName = cart.Selections[i].ProductName,
                    Price = cart.Selections[i].Price,
                    Quantity = cart.Selections[i].Quantity
                });
            }

            await publishEndpoint.Publish(eventMessage, cancellationToken);
            await service.DeleteCartAsync(cart.Id, cancellationToken);
            return new CheckoutBasketResult(checkoutCartRequest.CartCheckoutDto);
        }
    }
}
