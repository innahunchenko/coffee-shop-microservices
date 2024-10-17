using Microsoft.EntityFrameworkCore;

namespace Ordering.API.Data
{
    public static class DatabaseExtentions
    {
        public static void InitialiseDatabaseAsync(this WebApplication app)
        {
            using var scope = app.Services.CreateScope();

            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            context.Database.MigrateAsync().GetAwaiter().GetResult();
        }
    }
}
