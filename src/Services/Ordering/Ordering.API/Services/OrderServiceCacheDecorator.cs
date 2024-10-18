using Ordering.API.Domain.Dtos;

namespace Ordering.API.Services
{
    public class OrderServiceCacheDecorator : IOrderService
    {
        public Task Create(OrderDto order, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
