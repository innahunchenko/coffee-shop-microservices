using Ordering.API.Domain.Dtos;
using Ordering.API.Domain.Models;

namespace Ordering.API.Services
{
    public interface IOrderService
    {
        Task<Order> Create(OrderDto order, CancellationToken cancellationToken);
    }
}
