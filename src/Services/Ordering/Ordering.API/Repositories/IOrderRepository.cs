using Ordering.API.Domain.Models;
namespace Ordering.API.Repositories
{
    public interface IOrderRepository
    {
        Task<Order> CreateAsync(Order order, CancellationToken cancellationToken);
    }
}
