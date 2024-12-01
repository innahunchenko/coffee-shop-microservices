using Microsoft.EntityFrameworkCore;
using Ordering.API.Data;
using Ordering.API.Domain.Models;
using Ordering.API.Domain.ValueObjects.OrderObjects;

namespace Ordering.API.Repositories
{
    public class OrderRepository(IDbContext context) : IOrderRepository
    {
        public async Task<Order> CreateAsync(Order order, CancellationToken cancellationToken)
        {
            context.Orders.Add(order);
            await context.SaveChangesAsync(cancellationToken);

            var savedOrder = await context.Orders
                .Include(o => o.OrderItems)
                .FirstOrDefaultAsync(o => o.Id == order.Id, cancellationToken);

            if (savedOrder == null)
            {
                throw new InvalidOperationException($"Order with ID {order.Id} was not found after saving.");
            }

            return savedOrder;
        }

        public async Task<List<Order>> GetOrdersByEmailAsync(string email)
        {
            var orders = await context.Orders
                .Include(o => o.OrderItems)
                .Where(o => o.Email == Email.From(email))
                .ToListAsync();
            return orders;
        }

        public async Task<List<Order>> GetOrdersByUserIdAsync(string userId)
        {
            var orders = await context.Orders
                .Include(o => o.OrderItems)
                .Where(o => o.UserId == userId)
                .ToListAsync();
            return orders;
        }

        public async Task<List<Order>> GetAllOrdersAsync()
        {
            var orders = await context.Orders
                .Include(o => o.OrderItems)
                .ToListAsync();
            return orders;
        }
    }
}
