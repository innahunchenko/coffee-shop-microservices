using Foundation.Abstractions.Models;
using Foundation;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace Auth.API.Data.Configurations
{
    public sealed class OutboxMessageConsumerConfiguration : IEntityTypeConfiguration<OutboxMessageConsumer>
    {
        public void Configure(EntityTypeBuilder<OutboxMessageConsumer> builder)
        {
            builder.ToTable(TableNames.OutboxMessageConsumers);

            builder.HasKey(outboxMessageConsumer => new
            {
                outboxMessageConsumer.Id,
                outboxMessageConsumer.Name
            });
        }
    }
}
