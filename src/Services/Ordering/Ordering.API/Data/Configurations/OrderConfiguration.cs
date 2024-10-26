using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ordering.API.Domain.Models;
using Ordering.API.Domain.ValueObjects.OrderObjects;

namespace Ordering.API.Data.Configurations
{
    public class OrderConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.HasKey(o => o.Id);
            builder.Property(o => o.Id).HasConversion(orderId => orderId.Value, dbId => OrderId.From(dbId));
            builder.Property(o => o.OrderName).HasConversion(orderName => orderName.Value, dbOrderName => OrderName.From(dbOrderName));
            builder.HasMany(o => o.OrderItems).WithOne().HasForeignKey(oi => oi.OrderId);
            builder.Property(o => o.TotalPrice).IsRequired();
            builder.Property(o => o.PhoneNumber)
                .HasConversion(phoneNumber => phoneNumber.Value, dbPhoneNumber => PhoneNumber.From(dbPhoneNumber));
            builder.Property(o => o.Status).HasDefaultValue(OrderStatus.Draft)
                .HasConversion(s => s.ToString(), dbStatus => (OrderStatus)Enum.Parse(typeof(OrderStatus), dbStatus));

            builder.ComplexProperty(o => o.ShippingAddress, addressBuilder =>
            {
                addressBuilder.Property(a => a.FirstName).HasMaxLength(50).IsRequired();
                addressBuilder.Property(a => a.LastName).HasMaxLength(50).IsRequired();
                addressBuilder.Property(a => a.EmailAddress).HasMaxLength(50);
                addressBuilder.Property(a => a.AddressLine).HasMaxLength(180).IsRequired();
                addressBuilder.Property(a => a.Country).HasMaxLength(50).IsRequired();
                addressBuilder.Property(a => a.State).HasMaxLength(50).IsRequired();
                addressBuilder.Property(a => a.ZipCode).HasMaxLength(10).IsRequired();
            });

            builder.ComplexProperty(o => o.Payment, builder =>
            {
                builder.Property(a => a.CardName).HasMaxLength(50).IsRequired();
                builder.Property(a => a.CardNumber).HasMaxLength(19).IsRequired();
                builder.Property(a => a.Expiration).HasMaxLength(5).IsRequired();
                builder.Property(a => a.CVV).HasMaxLength(4).IsRequired();
            });
        }
    }
}
