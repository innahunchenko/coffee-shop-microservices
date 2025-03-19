using Marten;

namespace ShoppingCart.API.Services
{
    public class MartenWarmupService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;

        public MartenWarmupService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using var scope = _serviceProvider.CreateScope();
                var documentStore = scope.ServiceProvider.GetRequiredService<IDocumentStore>();

                try
                {
                    using var session = documentStore.LightweightSession();
                    await session.QueryAsync<int>("SELECT 1");
                    Console.WriteLine("Marten (Postgres) Warmup complete at: " + DateTime.UtcNow);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Marten Warmup failed: " + ex.Message);
                }

                await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
            }
        }
    }
}
