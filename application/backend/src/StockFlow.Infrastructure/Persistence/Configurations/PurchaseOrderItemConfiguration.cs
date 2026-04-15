using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StockFlow.Domain.Entities;

namespace StockFlow.Infrastructure.Persistence.Configurations;

public class PurchaseOrderItemConfiguration : IEntityTypeConfiguration<PurchaseOrderItem>
{
    public void Configure(EntityTypeBuilder<PurchaseOrderItem> builder)
    {
        builder.ToTable("PurchaseOrderItems");

        builder.HasKey(poi => poi.Id);

        builder.Property(poi => poi.Quantity)
            .IsRequired();

        builder.Property(poi => poi.UnitPrice)
            .HasPrecision(18, 2);

        builder.Property(poi => poi.Subtotal)
            .HasPrecision(18, 2);

        // Relationships are configured in PurchaseOrder configuration
    }
}
