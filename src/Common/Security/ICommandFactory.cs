using MediatR;
using Security.Models;

namespace Security
{
    public interface ICommandFactory
    {
        Type GetCommandTypeForRole(Roles role);
        IRequest<TResponse> CreateCommand<TResponse>(Type commandType);
    }
}
