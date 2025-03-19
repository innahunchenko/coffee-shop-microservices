namespace ShoppingCart.API.Services
{
    using Marten;
    using Microsoft.Extensions.Hosting;

    namespace Catalog.Infrastructure
    {
        public class MartenWarmupService : BackgroundService
        {
            private readonly IDocumentStore _documentStore;

            public MartenWarmupService(IDocumentStore documentStore)
            {
                _documentStore = documentStore;
            }

            protected override async Task ExecuteAsync(CancellationToken stoppingToken)
            {
                try
                {
                    using var session = _documentStore.LightweightSession();
                    await session.QueryAsync<int>("SELECT 1"); 
                    Console.WriteLine("Marten (Postgres) Warmup complete at: " + DateTime.UtcNow);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Marten Warmup failed: " + ex.Message);
                }
            }
        }
    }
}
