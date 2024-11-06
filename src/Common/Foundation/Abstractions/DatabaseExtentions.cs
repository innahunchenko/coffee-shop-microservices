using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Foundation.Abstractions
{
    public static class DatabaseExtentions
    {
        public static void InitialiseDatabaseAsync<TContext>(this WebApplication app) where TContext : DbContext
        {
            using var scope = app.Services.CreateScope();

            var context = scope.ServiceProvider.GetRequiredService<TContext>();

            context.Database.MigrateAsync().GetAwaiter().GetResult();
        }
    }
}
