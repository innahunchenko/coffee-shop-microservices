using MediatR;
using Ordering.API.Domain.Dtos;

namespace Ordering.API.Orders.Commands.CreateOrder
{
    public record CreateOrderRequest(OrderDto OrderDto) : IRequest<OrderDto>;

    public class CreateOrderHandler : IRequestHandler<CreateOrderRequest, OrderDto>
    {
        public Task<OrderDto> Handle(CreateOrderRequest request, CancellationToken cancellationToken)
        {
            //var order = CreateOrder(request.OrderDto);
            throw new NotImplementedException();
        }
    }
}
