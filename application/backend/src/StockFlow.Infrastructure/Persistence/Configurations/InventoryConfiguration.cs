using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StockFlow.Domain.Entities;

namespace StockFlow.Infrastructure.Persistence.Configurations;

public class InventoryConfiguration : IEntityTypeConfiguration<Inventory>
{
    public void Configure(EntityTypeBuilder<Inventory> builder)
    {
        builder.ToTable("Inventories");

        builder.HasKey(i => i.Id);

        builder.Property(i => i.Quantity)
            .IsRequired();

        builder.Property(i => i.Threshold)
            .IsRequired();

        builder.Property(i => i.LastUpdated)
            .IsRequired();

        // Create unique index on ProductId and WarehouseId combination
        builder.HasIndex(i => new { i.ProductId, i.WarehouseId })
            .IsUnique();

        // Relationships are configured in Product and Warehouse configurations
    }
}
