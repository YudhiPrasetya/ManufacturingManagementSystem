
using Moq;
using Production.Application.Interfaces;
using Production.Application.UseCases.ProductionOrders.ReportProduction;
using Production.Domain.Entities;

namespace Production.Tests;

public class ReportProductionHandlerTests
{
    [Fact]
    public async Task Handle_ShouldUpdateProductionOrder_WhenOrderExists()
    {
        // Arrange
        var repositoryMock = new Mock<IProductionOrderRepository>();

        var productionOrder = new ProductionOrder(
            "PO-TEST-001",
            "FAB-001",
            "Cotton Fabric 30s",
            1000
        );

        repositoryMock
            .Setup(r => r.GetByOrderNumberAsync(
                "PO-TEST-001",
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(productionOrder);

        var handler = new ReportProductionHandler(repositoryMock.Object);

        var command = new ReportProductionCommand("PO-TEST-001", 250);

        // Act
        await handler.Handle(command);

        // Assert
        Assert.Equal(250, productionOrder.CompletedQuantity);
        Assert.Equal("InProgress", productionOrder.Status);

        repositoryMock.Verify(
            r => r.UpdateAsync(
                productionOrder,
                It.IsAny<CancellationToken>()
            ),
            Times.Once
        );
    }

    [Fact]
    public async Task Handle_ShouldThrowKeyNotFoundException_WhenOrderDoesNotExist()
    {
        // Arrange
        var repositoryMock = new Mock<IProductionOrderRepository>();

        repositoryMock
            .Setup(r => r.GetByOrderNumberAsync(
                "PO-UNKNOWN",
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((ProductionOrder?)null);

        var handler = new ReportProductionHandler(repositoryMock.Object);

        var command = new ReportProductionCommand("PO-UNKNOWN", 250);

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(
            () => handler.Handle(command)
        );

        repositoryMock.Verify(
            r => r.UpdateAsync(
                It.IsAny<ProductionOrder>(),
                It.IsAny<CancellationToken>()),
            Times.Never
        );
    }

}
