using Ordering.API.Domain.Dtos;
namespace Ordering.API.Services
{
    public interface IOrderService
    {
        Task<OrderDto> CreateAsync(OrderDto order, CancellationToken cancellationToken);
        Task<List<OrderDto>> GetAllOrdersAsync();
        Task<List<OrderDto>> GetOrdersByLoggedInUserAsync(string userId, string email);
    }
}
