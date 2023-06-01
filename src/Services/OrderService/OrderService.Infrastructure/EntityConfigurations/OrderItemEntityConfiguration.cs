using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using OrderService.Domain.AggregateModels.OrderAggregate;
using OrderService.Infrastructure.Context;

namespace OrderService.Infrastructure.EntityConfigurations
{
    public class OrderItemEntityConfiguration : IEntityTypeConfiguration<OrderItem>
    {
        public void Configure(EntityTypeBuilder<OrderItem> builder)
        {
            builder.ToTable("orderItems", OrderDbContext.DEFAULT_SCHEMA);

            builder.HasKey(o => o.Id);

            builder.Ignore(b => b.DomainEvents);

            builder.Property(o => o.Id).ValueGeneratedOnAdd();

            builder.Property<int>("OrderId").IsRequired();
        }
    }
}
