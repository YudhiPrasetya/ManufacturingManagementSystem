using Microsoft.EntityFrameworkCore;
using Production.Domain.Entities;

namespace Production.Infrastructure.Persistence;

public class ProductionDbContext : DbContext
{
    public ProductionDbContext(
        DbContextOptions<ProductionDbContext> options) : base(options)
    {

    }

    public DbSet<ProductionOrder> ProductionOrders => Set<ProductionOrder>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<ProductionOrder>(entity =>
        {
            entity.ToTable("ProductionOrders");

            entity.HasKey(x => x.Id);

            entity.Property(x => x.OrderNumber).HasMaxLength(50).IsRequired();
            entity.HasIndex(x => x.OrderNumber).IsUnique();

            entity.Property(x => x.ProductCode).HasMaxLength(50).IsRequired();

            entity.Property(x => x.ProductName).HasMaxLength(200).IsRequired();

            entity.Property(x => x.PlannedQuantity).IsRequired();

            entity.Property(x => x.CompletedQuantity).IsRequired();

            entity.Property(x => x.Status).HasMaxLength(30).IsRequired();
        });
    }

}
