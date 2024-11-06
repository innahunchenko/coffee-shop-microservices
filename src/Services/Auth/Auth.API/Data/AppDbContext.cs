using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Auth.API.Domain.Models;
using Foundation.Abstractions;
using Auth.API.Data.Configurations;

namespace Auth.API.Data
{
    public interface IDbContext : IAppDbContext
    {
        DbSet<CoffeeShopUser> CoffeeShopUsers { get; }
    }

    public class AppDbContext : IdentityDbContext<CoffeeShopUser>, IDbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<CoffeeShopUser> CoffeeShopUsers => Set<CoffeeShopUser>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfiguration(new CoffeeShopUserConfiguration());
        }
    }
}
