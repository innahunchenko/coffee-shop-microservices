using Foundation.Abstractions.Models;

namespace Auth.API.Repositories
{
    public interface IOutboxRepository
    {
        Task CreateAsync(BaseEvent baseEvent);
    }
}
