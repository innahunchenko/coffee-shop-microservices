using Auth.API.Domain.Models;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace Auth.API.Data.Configurations
{
    public class CoffeeShopUserConfiguration : IEntityTypeConfiguration<CoffeeShopUser>
    {
        public void Configure(EntityTypeBuilder<CoffeeShopUser> builder)
        {
            builder.HasIndex(u => u.PhoneNumber).IsUnique();
        }
    }
}
