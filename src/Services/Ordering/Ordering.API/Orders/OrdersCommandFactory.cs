using MediatR;
using Ordering.API.Orders.Queries.GetOrders;
using Security.Models;
using Security;

namespace Ordering.API.Orders
{
    public class OrdersCommandFactory : ICommandFactory
    {
        private static readonly Dictionary<Roles, Type> RoleCommandMapping = new()
        {
            { Roles.USER, typeof(GetOrdersByUserRequest) },
            { Roles.ADMIN, typeof(GetOrdersByAdminRequest) }
        };

        public Type GetCommandTypeForRole(Roles role)
        {
            if (!RoleCommandMapping.TryGetValue(role, out var commandType))
            {
                throw new UnauthorizedAccessException("Invalid role.");
            }

            return commandType;
        }

        public IRequest<TResponse> CreateCommand<TResponse>(Type commandType)
        {
            return Activator.CreateInstance(commandType) as IRequest<TResponse>
                ?? throw new InvalidOperationException("Unable to create command instance.");
        }
    }

}
