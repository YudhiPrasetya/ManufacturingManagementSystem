using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Production.Domain.Entities;
using Production.Infrastructure.Persistence;
using Production.Infrastructure.Repositories;

namespace Production.Tests.Infrastructure;

public class ProductionOrderRepositoryTests
{
    private static ProductionDbContext CreateDbContext(SqliteConnection connection)
    {
        var options = new DbContextOptionsBuilder<ProductionDbContext>()
            .UseSqlite(connection).Options;

        return new ProductionDbContext(options);
    }

    [Fact]
    public async Task AddAsync_ShouldSaveProductionOrderToDatabase()
    {
        // Arrange
        await using var connection = new SqliteConnection("Data Source=:memory:");

        await connection.OpenAsync();

        await using var context = CreateDbContext(connection);

        await context.Database.EnsureCreatedAsync();

        var repository = new ProductionOrderRepository(context);

        var productionOrder = new ProductionOrder(
            "PO-TEST-001",
            "FAB-001",
            "Cotton Fabric 30s",
            1000
        );

        // Act
        await repository.AddAsync(productionOrder);

        // Assert
        var savedOrder = await context.ProductionOrders
            .FirstOrDefaultAsync(x => x.OrderNumber == "PO-TEST-001");

        Assert.NotNull(savedOrder);
        Assert.Equal("FAB-001", savedOrder.ProductCode);
        Assert.Equal(1000, savedOrder.PlannedQuantity);
        Assert.Equal("Planned", savedOrder.Status);
    }

    [Fact]
    public async Task GetByOrderNumberAsync_ShouldReturnProductionOrder_WhenOrderExists()
    {
        // Arrange
        await using var connection = new SqliteConnection(
            "Data Source=:memory:");

        await connection.OpenAsync();

        await using var context = CreateDbContext(connection);

        await context.Database.EnsureCreatedAsync();

        var repository = new ProductionOrderRepository(context);

        var productionOrder = new ProductionOrder(
            "PO-TEST-002",
            "FAB-002",
            "Polyester Fabric",
            2000
        );

        await context.ProductionOrders.AddAsync(productionOrder);
        await context.SaveChangesAsync();

        // Act
        var result = await repository.GetByOrderNumberAsync("PO-TEST-002");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("PO-TEST-002", result.OrderNumber);
        Assert.Equal("FAB-002", result.ProductCode);
        Assert.Equal(2000, result.PlannedQuantity);
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateProductionOrderInDatabase()
    {
        // Arrange
        await using var connection = new SqliteConnection("Data Source=:memory:");

        await connection.OpenAsync();

        await using var context = CreateDbContext(connection);

        await context.Database.EnsureCreatedAsync();

        var repository = new ProductionOrderRepository(context);

        var productionOrder = new ProductionOrder(
            "PO-TEST-003",
            "FAB-003",
            "Cotton Fabric 40s",
            1000
        );

        await repository.AddAsync(productionOrder);

        // Act
        productionOrder.ReportProduction(400);

        await repository.UpdateAsync(productionOrder);

        // Assert
        var updateOrder = await context.ProductionOrders.AsNoTracking()
                                    .FirstOrDefaultAsync(x => x.OrderNumber == "PO-TEST-003");

        Assert.NotNull(updateOrder);
        Assert.Equal(400, updateOrder.CompletedQuantity);
        Assert.Equal("InProgress", updateOrder.Status);
    }

    [Fact]
    public async Task GetPagedAsync_ShouldReturnPagedProductionOrders()
    {
        // Arrange
        await using var connection = new SqliteConnection("Data Source=:memory:");

        await connection.OpenAsync();

        await using var context = CreateDbContext(connection);

        await context.Database.EnsureCreatedAsync();

        var repository = new ProductionOrderRepository(context);

        var orders = new[]
        {
            new ProductionOrder("PO-TEST-004", "FAB-004", "Product A", 1000),
            new ProductionOrder("PO-TEST-005", "FAB-005", "Product B", 2000),
            new ProductionOrder("PO-TEST-006", "FAB-006", "Product C", 3000)
        };

        await context.ProductionOrders.AddRangeAsync(orders);
        await context.SaveChangesAsync();

        // Act
        var (items, totalCount) = await repository.GetPagedAsync(page: 1, pageSize: 2);

        // Assert
        Assert.Equal(3, totalCount);
        Assert.Equal(2, items.Count);
        Assert.Equal("PO-TEST-006", items[0].OrderNumber);
        Assert.Equal("PO-TEST-005", items[1].OrderNumber);
    }

    [Fact]
    public async Task GetPagedAsync_ShouldFilterOrdersByStatus()
    {
        // Arrange
        await using var connection = new SqliteConnection("Data Source=:memory:");

        await connection.OpenAsync();

        await using var context = CreateDbContext(connection);

        await context.Database.EnsureCreatedAsync();

        var repository = new ProductionOrderRepository(context);

        var completedOrder = new ProductionOrder(
            "PO-TEST-007",
            "FAB-007",
            "Product A",
            1000
        );

        completedOrder.ReportProduction(1000);

        var inProgressOrder = new ProductionOrder(
            "PO-TEST-008",
            "FAB-008",
            "Product B",
            2000
        );

        inProgressOrder.ReportProduction(500);

        var plannedOrder = new ProductionOrder(
            "PO-TEST-009",
            "FAB-009",
            "Product C",
            3000
        );

        await context.ProductionOrders.AddRangeAsync(
            completedOrder,
            inProgressOrder,
            plannedOrder
        );

        await context.SaveChangesAsync();

        // Act
        var (items, totalCount) = await repository.GetPagedAsync(
            page: 1,
            pageSize: 10,
            status: "Completed"
        );

        // Assert
        Assert.Equal(1, totalCount);
        Assert.Single(items);
        Assert.Equal("PO-TEST-007", items[0].OrderNumber);
        Assert.Equal("Completed", items[0].Status);
    }

    [Fact]
    public async Task AddAsync_ShouldThrowException_WhenOrderNumberAlreadyExists()
    {
        await using var connection = new SqliteConnection("Data Source=:memory:");

        await connection.OpenAsync();

        await using var context = CreateDbContext(connection);

        await context.Database.EnsureCreatedAsync();

        var repository = new ProductionOrderRepository(context);

        var firstOrder = new ProductionOrder(
            "PO-TEST-010",
            "FAB-010",
            "Product A",
            1000
        );

        var duplicateOrder = new ProductionOrder(
            "PO-TEST-010",
            "FAB-011",
            "Product B",
            2000
        );

        await repository.AddAsync(firstOrder);

        // Act and Assert
        await Assert.ThrowsAnyAsync<DbUpdateException>(
            () => repository.AddAsync(duplicateOrder)
        );
    }

}
