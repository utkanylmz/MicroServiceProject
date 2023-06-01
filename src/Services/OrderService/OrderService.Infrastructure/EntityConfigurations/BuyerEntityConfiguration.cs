using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using OrderService.Infrastructure.Context;
using OrderService.Domain.AggregateModels.BuyerAggregate;

namespace OrderService.Infrastructure.EntityConfigurations
{
    public class BuyerEntityConfiguration : IEntityTypeConfiguration<Buyer>
    {
        public void Configure(EntityTypeBuilder<Buyer> builder)
        {
            builder.ToTable("buyers", OrderDbContext.DEFAULT_SCHEMA);

            builder.HasKey(b => b.Id);

            builder.Ignore(b => b.DomainEvents);
            builder.Property(b => b.Id).ValueGeneratedOnAdd();

            builder.Property(b => b.Name).HasColumnName("name").HasColumnType("varchar").HasMaxLength(100);

            builder.HasMany(b => b.PaymentMethods)
                .WithOne()
                .HasForeignKey(x => x.Id)
                .OnDelete(DeleteBehavior.Cascade);

            var navigation = builder.Metadata.FindNavigation(nameof(Buyer.PaymentMethods));

            navigation.SetPropertyAccessMode(PropertyAccessMode.Field);
        }
    }
}
