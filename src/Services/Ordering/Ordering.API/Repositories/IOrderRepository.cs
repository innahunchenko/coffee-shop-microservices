using Ordering.API.Domain.Models;
namespace Ordering.API.Repositories
{
    public interface IOrderRepository
    {
        Task<Order> Create(Order order, CancellationToken cancellationToken);
    }
}
