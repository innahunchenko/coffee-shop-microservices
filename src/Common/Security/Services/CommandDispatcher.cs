using MediatR;

namespace Security.Services
{
    public class CommandDispatcher
    {
        private readonly IMediator mediator;
        private readonly IUserContext userContext;
        private readonly ICommandFactory commandFactory;

        public CommandDispatcher(
            IMediator mediator, 
            IUserContext userContext, 
            ICommandFactory commandFactory)
        {
            this.mediator = mediator;
            this.userContext = userContext;
            this.commandFactory = commandFactory;
        }

        public async Task<TResponse> DispatchAsync<TResponse>()
        {
            var role = userContext.GetUserRole();

            var commandType = commandFactory.GetCommandTypeForRole(role);

            var command = commandFactory.CreateCommand<TResponse>(commandType);

            return await mediator.Send(command);
        }
    }
}
