using MediatR;
using Ordering.API.Domain.Dtos;
using Ordering.API.Services;

namespace Ordering.API.Orders.Queries.GetOrders
{
    public record GetOrdersByAdminRequest() : IRequest<List<OrderDto>>;

    public class GetOrdersByAdminHandler(IOrderService orderService) : IRequestHandler<GetOrdersByAdminRequest, List<OrderDto>>
    {
        public async Task<List<OrderDto>> Handle(GetOrdersByAdminRequest request, CancellationToken cancellationToken)
        {
            var ordersDto = await orderService.GetAllOrdersAsync();
            return ordersDto;
        }
    }
}
