using Auth.API.Data;
using Foundation.Abstractions.Models;
using Newtonsoft.Json;

namespace Auth.API.Repositories
{
    public class OutboxRepository : IOutboxRepository
    {
        private readonly AppDbContext appDbContext;
        public OutboxRepository(AppDbContext appDbContext)
        {
            this.appDbContext = appDbContext;
        }

        public async Task CreateAsync(BaseEvent baseEvent)
        {
            var outboxMessage = new OutboxMessage
            {
                Id = Guid.NewGuid(),
                OccurredOnUtc = DateTime.UtcNow,
                Type = baseEvent.GetType().Name,
                Content = JsonConvert.SerializeObject(
                    baseEvent,
                    new JsonSerializerSettings
                    {
                        TypeNameHandling = TypeNameHandling.All
                    })
            };

            await appDbContext.Outbox.AddAsync(outboxMessage);
        }
    }
}
