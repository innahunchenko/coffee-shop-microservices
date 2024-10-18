using MassTransit;
using MediatR;
using Messaging.Events;
using Ordering.API.Mapping;
using Ordering.API.Orders.Commands.CreateOrder;

namespace Ordering.API.Orders.EventHandlers.Integration
{
    public class CartCheckoutEventHandler(ISender sender, ILogger<CartCheckoutEventHandler> logger) : IConsumer<CartCheckoutEvent>
    {
        public async Task Consume(ConsumeContext<CartCheckoutEvent> context)
        {
            logger.LogInformation("Integration Event handled: {IntegrationEvent}", context.Message.GetType().Name);
            var request = new CreateOrderRequest(context.Message.ToOrderDto());
            await sender.Send(request);
        }
    }
}
