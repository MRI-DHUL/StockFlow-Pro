using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StockFlow.Domain.Entities;

namespace StockFlow.Infrastructure.Persistence.Configurations;

public class WarehouseConfiguration : IEntityTypeConfiguration<Warehouse>
{
    public void Configure(EntityTypeBuilder<Warehouse> builder)
    {
        builder.ToTable("Warehouses");

        builder.HasKey(w => w.Id);

        builder.Property(w => w.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(w => w.Location)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(w => w.ContactInfo)
            .HasMaxLength(500);

        builder.Property(w => w.Email)
            .HasMaxLength(100);

        builder.Property(w => w.Phone)
            .HasMaxLength(20);

        // Relationships
        builder.HasMany(w => w.Inventories)
            .WithOne(i => i.Warehouse)
            .HasForeignKey(i => i.WarehouseId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(w => w.StockMovementsFrom)
            .WithOne(sm => sm.FromWarehouse)
            .HasForeignKey(sm => sm.FromWarehouseId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(w => w.StockMovementsTo)
            .WithOne(sm => sm.ToWarehouse)
            .HasForeignKey(sm => sm.ToWarehouseId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
