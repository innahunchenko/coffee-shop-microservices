using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Auth.API.Domain.Models;
using Auth.API.Data.Configurations;
using Foundation.DbConfigurations;
using Foundation.Abstractions.Models;

namespace Auth.API.Data
{
    public class AppDbContext : IdentityDbContext<CoffeeShopUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<OutboxMessage> Outbox { get; set; } = default!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfiguration(new CoffeeShopUserConfiguration());
            modelBuilder.ApplyConfiguration(new OutboxMessageConfiguration());
            modelBuilder.ApplyConfiguration(new OutboxMessageConsumerConfiguration());
        }
    }
}
