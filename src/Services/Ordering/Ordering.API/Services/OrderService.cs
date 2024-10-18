using Ordering.API.Domain.Dtos;
using Ordering.API.Domain.Models;
using Ordering.API.Mapping;
using Ordering.API.Repositories;

namespace Ordering.API.Services
{
    public class OrderService(IOrderRepository repository) : IOrderService
    {
        public async Task<Order> Create(OrderDto orderDto, CancellationToken cancellationToken)
        {
            var order = orderDto.ToOrder();
            order = await repository.Create(order, cancellationToken);
            return order;
        }
    }
}
