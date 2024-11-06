using Ordering.API.Domain.Dtos;
using Ordering.API.Domain.Models;

namespace Ordering.API.Services
{
    public class OrderServiceCacheDecorator : IOrderService
    {
        public Task<Order> CreateAsync(OrderDto order, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
