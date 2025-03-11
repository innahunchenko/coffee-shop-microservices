using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ordering.API.Domain.Models;
using Ordering.API.Domain.ValueObjects.OrderItemObjects;
using Ordering.API.Domain.ValueObjects.OrderObjects;

namespace Ordering.API.Data.Configurations
{
    public class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
    {
        public void Configure(EntityTypeBuilder<OrderItem> builder)
        {
            builder.HasKey(oi => oi.Id);

            builder.Property(oi => oi.Id)
                .HasConversion(
                    orderItemId => orderItemId.Value, 
                    dbId => OrderItemId.From(dbId)); 

            builder.Property(oi => oi.OrderId)
                .HasConversion(
                    orderId => orderId.Value, 
                    value => OrderId.From(value));

            builder.Property(oi => oi.ProductId)
                .HasConversion(
                    productId => productId.Value,
                    value => ProductId.From(value));

            builder.Property(oi => oi.ProductName)
                .HasMaxLength(50)
                .IsRequired()
                .HasConversion(
                    productName => productName.Value, 
                    value => ProductName.From(value));
            builder.Property(o => o.Price).HasColumnType("decimal(18,2)");

            builder.Property(oi => oi.Quantity).IsRequired();
            builder.Property(oi => oi.Price).IsRequired();
        }
    }
}
