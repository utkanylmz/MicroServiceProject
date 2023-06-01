using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using OrderService.Domain.AggregateModels.OrderAggregate;
using OrderService.Infrastructure.Context;

namespace OrderService.Infrastructure.EntityConfigurations
{
    public class OrderStatusEntityConfiguration : IEntityTypeConfiguration<OrderStatus>
    {
        public void Configure(EntityTypeBuilder<OrderStatus> builder)
        {
            builder.ToTable("orderstatus", OrderDbContext.DEFAULT_SCHEMA);

            builder.HasKey(o => o.Id);
            builder.Property(i => i.Id).ValueGeneratedOnAdd();

            builder.Property(o => o.Id)
                   .HasDefaultValue(1)
                   .ValueGeneratedNever()
                   .IsRequired();

            builder.Property(o => o.Name).HasMaxLength(200).IsRequired();
        }
    }
}
