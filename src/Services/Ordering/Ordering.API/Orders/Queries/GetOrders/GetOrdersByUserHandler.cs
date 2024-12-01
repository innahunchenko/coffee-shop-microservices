using MediatR;
using Ordering.API.Domain.Dtos;
using Ordering.API.Services;
using Security.Services;

namespace Ordering.API.Orders.Queries.GetOrders
{
    public record GetOrdersByUserRequest() : IRequest<List<OrderDto>>;

    public class GetOrdersByUserHandler(IOrderService orderService, IUserContext userContext) : IRequestHandler<GetOrdersByUserRequest, List<OrderDto>>
    {
        public async Task<List<OrderDto>> Handle(GetOrdersByUserRequest request, CancellationToken cancellationToken)
        {
            var email = userContext.GetUserEmail();
            var userId = userContext.GetUserId();
            var ordersDto = await orderService.GetOrdersByLoggedInUserAsync(userId!, email);
            return ordersDto;
        }
    }
}
