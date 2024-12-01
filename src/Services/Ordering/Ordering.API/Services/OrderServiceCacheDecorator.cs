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

        public Task<List<OrderDto>> GetAllOrdersAsync()
        {
            throw new NotImplementedException();
        }

        public Task<List<OrderDto>> GetOrdersByLoggedInUserAsync(string userId, string email)
        {
            throw new NotImplementedException();
        }

        Task<OrderDto> IOrderService.CreateAsync(OrderDto order, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
