using Ordering.API.Domain.Dtos;
using Ordering.API.Mapping;
using Ordering.API.Repositories;

namespace Ordering.API.Services
{
    public class OrderService(IOrderRepository repository) : IOrderService
    {
        public async Task<OrderDto> CreateAsync(OrderDto orderDto, CancellationToken cancellationToken)
        {
            var order = orderDto.ToOrder();
            order = await repository.CreateAsync(order, cancellationToken);
            orderDto = order.ToOrderDto();
            return orderDto;
        }

        public async Task<List<OrderDto>> GetAllOrdersAsync()
        {
            var orders = await repository.GetAllOrdersAsync();
            var ordersDto = orders.Select(order => order.ToOrderDto()).ToList();
            return ordersDto;
        }

        public async Task<List<OrderDto>> GetOrdersByLoggedInUserAsync(string userId, string email)
        {
            var ordersByEmail = await repository.GetOrdersByEmailAsync(email);
            var ordersDto = ordersByEmail.Select(order => order.ToOrderDto()).ToList();
            return ordersDto;
        }
    }
}
