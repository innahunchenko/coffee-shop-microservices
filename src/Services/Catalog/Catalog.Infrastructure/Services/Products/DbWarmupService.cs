using Dapper;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Data;

namespace Catalog.Infrastructure.Services.Products
{
    public class DbWarmupService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;

        public DbWarmupService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using var scope = _serviceProvider.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<IDbConnection>();

                try
                {
                    await db.ExecuteAsync("SELECT 1");
                    Console.WriteLine("DB Warmed up at: " + DateTime.UtcNow);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Warmup failed: " + ex.Message);
                }

                await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
            }
        }
    }

}
