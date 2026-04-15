using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StockFlow.Domain.Entities;

namespace StockFlow.Infrastructure.Persistence.Configurations;

public class PurchaseOrderConfiguration : IEntityTypeConfiguration<PurchaseOrder>
{
    public void Configure(EntityTypeBuilder<PurchaseOrder> builder)
    {
        builder.ToTable("PurchaseOrders");

        builder.HasKey(po => po.Id);

        builder.Property(po => po.PONumber)
            .IsRequired()
            .HasMaxLength(50);

        builder.HasIndex(po => po.PONumber)
            .IsUnique();

        builder.Property(po => po.Status)
            .IsRequired();

        builder.Property(po => po.ExpectedDeliveryDate)
            .IsRequired();

        builder.Property(po => po.TotalAmount)
            .HasPrecision(18, 2);

        // Relationships
        builder.HasMany(po => po.PurchaseOrderItems)
            .WithOne(poi => poi.PurchaseOrder)
            .HasForeignKey(poi => poi.PurchaseOrderId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
