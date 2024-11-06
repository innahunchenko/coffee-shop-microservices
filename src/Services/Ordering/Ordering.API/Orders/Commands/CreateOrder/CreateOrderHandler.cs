using MediatR;
using Ordering.API.Domain.Dtos;
using Ordering.API.Mapping;
using Ordering.API.Services;

namespace Ordering.API.Orders.Commands.CreateOrder
{
    public record CreateOrderRequest(OrderDto OrderDto) : IRequest<OrderDto>;

    public class CreateOrderHandler(IOrderService orderService) : IRequestHandler<CreateOrderRequest, OrderDto>
    {
        public async Task<OrderDto> Handle(CreateOrderRequest request, CancellationToken cancellationToken)
        {
            var order = await orderService.CreateAsync(request.OrderDto, cancellationToken);
            var orderDto = order.ToOrderDto();
            return orderDto;
        }
    }
}
