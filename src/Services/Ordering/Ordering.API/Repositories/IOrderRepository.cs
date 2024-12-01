using Ordering.API.Domain.Models;
namespace Ordering.API.Repositories
{
    public interface IOrderRepository
    {
        Task<Order> CreateAsync(Order order, CancellationToken cancellationToken);
        Task<List<Order>> GetAllOrdersAsync();
        Task<List<Order>> GetOrdersByEmailAsync(string email);
        Task<List<Order>> GetOrdersByUserIdAsync(string userId);
    }
}
