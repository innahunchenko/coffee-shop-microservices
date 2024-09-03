using Catalog.Domain.Models;
using Catalog.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Catalog.Infrastructure.Configurations
{
    public class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.HasKey(p => p.Id);

            builder.Property(p => p.Id)
                .HasConversion(productId => productId.Value, dbId => ProductId.Of(dbId));

            builder.Property(p => p.CategoryId)
                .HasConversion(
                    categoryId => categoryId.Value,
                    dbId => CategoryId.Of(dbId))
                .IsRequired();

            builder.Property(p => p.Name)
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(p => p.Price)
                .HasPrecision(18, 2);

            builder.Property(p => p.Description)
                .HasMaxLength(100);

            builder.HasOne(p => p.Category)
                .WithMany()
                .HasForeignKey(p => p.CategoryId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
