using Ordering.API.Domain.Dtos;
using Ordering.API.Domain.Models;
using Ordering.API.Mapping;
using Ordering.API.Repositories;

namespace Ordering.API.Services
{
    public class OrderService(IOrderRepository repository) : IOrderService
    {
        public async Task<Order> CreateAsync(OrderDto orderDto, CancellationToken cancellationToken)
        {
            var order = orderDto.ToOrder();
            order = await repository.CreateAsync(order, cancellationToken);
            return order;
        }
    }
}
