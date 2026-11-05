using Production.Domain.Entities;

namespace Production.Tests;

public class ProductionOrderTests
{
    [Fact]
    public void Constructor_WithValidData_ShouldCreateProductionOrder()
    {
        // Arrange and Act
        var order = new ProductionOrder(
            "PO-TEST-001",
            "FAB-001",
            "Cotton Fabric 30s",
            1000
        );

        // Assert
        Assert.NotEqual(Guid.Empty, order.Id);
        Assert.Equal("PO-TEST-001", order.OrderNumber);
        Assert.Equal("FAB-001", order.ProductCode);
        Assert.Equal("Cotton Fabric 30s", order.ProductName);
        Assert.Equal(1000, order.PlannedQuantity);
        Assert.Equal(0, order.CompletedQuantity);
        Assert.Equal("Planned", order.Status);
        Assert.Equal(DateTimeKind.Utc, order.CreatedAt.Kind);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void Constructor_WithEmptyProductName_ShouldThrowArgumentException(string productName)
    {
        Assert.Throws<ArgumentException>(() =>
            new ProductionOrder("PO-TEST-001", "FAB-001", productName, 1000)
        );
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Constructor_WithInvalidPlannedQuantity_ShouldThrowArgumentException(int plannedQuantity)
    {
        Assert.Throws<ArgumentException>(() =>
            new ProductionOrder(
                "PO-TEST-001", "FAB-001", "Cotton Fabric", plannedQuantity
            )
        );
    }

    [Fact]
    public void ReportProduction_WithPartialQuantity_ShouldSetInProgressStatus()
    {
        // Arrange
        var order = CreateProductionOrder();

        // Act
        order.ReportProduction(400);

        // Assert
        Assert.Equal(400, order.CompletedQuantity);
        Assert.Equal("InProgress", order.Status);

    }

    [Fact]
    public void ReportProduction_WithRemainingQuantity_ShouldSetCompletedStatus()
    {
        // Arrange
        var order = CreateProductionOrder();
        order.ReportProduction(400);

        // Act
        order.ReportProduction(600);

        // Assert
        Assert.Equal(1000, order.CompletedQuantity);
        Assert.Equal("Completed", order.Status);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void ReportProduction_WithInvalidQuantity_ShouldThrowArgumentException(int quantity)
    {
        // Arrange
        var order = CreateProductionOrder();

        // Act and Assert
        Assert.Throws<ArgumentException>(() =>
            order.ReportProduction(quantity)
        );
    }

    [Fact]
    public void ReportProduction_ExceedingPlannedQuantity_ShouldThrowInvalidOpertaionException()
    {
        // Arrange
        var order = CreateProductionOrder();
        order.ReportProduction(800);

        // Act
        Assert.Throws<InvalidOperationException>(() =>
            order.ReportProduction(300)
        );

        // Assert: state tidak berubah setelah operasi gagal
        Assert.Equal(800, order.CompletedQuantity);
        Assert.Equal("InProgress", order.Status);
    }

    private static ProductionOrder CreateProductionOrder()
    {
        return new ProductionOrder(
            "PO-TEST-001",
            "FAB-001",
            "Cotton Fabric 30s",
            1000
        );
    }



}
