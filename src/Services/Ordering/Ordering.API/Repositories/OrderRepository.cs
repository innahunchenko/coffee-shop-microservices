using Microsoft.EntityFrameworkCore;
using Ordering.API.Data;
using Ordering.API.Domain.Models;

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
    }
}
