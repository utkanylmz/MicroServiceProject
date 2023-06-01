using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using OrderService.Infrastructure.Context;
using OrderService.Domain.AggregateModels.BuyerAggregate;

namespace OrderService.Infrastructure.EntityConfigurations
{
    public class CardTypeEntityConfiguration : IEntityTypeConfiguration<CardType>
    {
        public void Configure(EntityTypeBuilder<CardType> builder)
        {
            builder.ToTable("cardtypes", OrderDbContext.DEFAULT_SCHEMA);

            builder.HasKey(ct => ct.Id);
            builder.Property(i => i.Id).HasColumnName("id").ValueGeneratedOnAdd();

            builder.Property(ct => ct.Id)
                   .HasDefaultValue(1)
                   .ValueGeneratedNever()
                   .IsRequired();

            builder.Property(ct => ct.Name).HasMaxLength(200).IsRequired();
        }
    }
}
