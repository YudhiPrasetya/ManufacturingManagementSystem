using Moq;
using Production.Application.Interfaces;
using Production.Application.UseCases.ProductionOrders.CreateProductionOrder;
using Production.Domain.Entities;

namespace Production.Tests;

public class CreateProductionOrderHandlerTests
{
    [Fact]
    public async Task Handle_ShouldCreateProductionOrder_WhenOrderNumberDoesNotExist()
    {
        // Arrange
        var repositoryMock = new Mock<IProductionOrderRepository>();

        var command = new CreateProductionOrderCommand(
            "PO-TEST-001",
            "FAB-001",
            "Cotton Fabric 30s",
            100
        );

        repositoryMock
            .Setup(r => r.GetByOrderNumberAsync(
                command.OrderNumber,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((ProductionOrder?)null);

        repositoryMock
            .Setup(r => r.AddAsync(
                It.IsAny<ProductionOrder>(),
                It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var handler = new CreateProductionOrderHandler(
            repositoryMock.Object
        );

        // Act
        var result = await handler.Handle(command);

        // Assert
        Assert.NotEqual(Guid.Empty, result);

        repositoryMock.Verify(
            r => r.AddAsync(
                It.Is<ProductionOrder>(p =>
                    p.OrderNumber == command.OrderNumber &&
                    p.ProductCode == command.ProductCode &&
                    p.ProductName == command.ProductName &&
                    p.PlannedQuantity == command.PlannedQuantity),
                It.IsAny<CancellationToken>()),
            Times.Once
        );
    }

    [Fact]
    public async Task Handle_ShouldThrowInvalidOperationException_WhenOrderNumberAlreadyExists()
    {
        // Arrange
        var repositoryMock = new Mock<IProductionOrderRepository>();

        var command = new CreateProductionOrderCommand(
            "PO-TEST-001",
            "FAB-001",
            "Cotton Fabric 30s",
            1000
        );

        var existingOrder = new ProductionOrder(
            "PO-TEST-001",
            "FAB-001",
            "Cotton Fabric 30s",
            1000
        );

        repositoryMock
            .Setup(r => r.GetByOrderNumberAsync(
                command.OrderNumber,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingOrder);

        var handler = new CreateProductionOrderHandler(
            repositoryMock.Object
        );

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => handler.Handle(command)
        );

        repositoryMock.Verify(
            r => r.AddAsync(
                It.IsAny<ProductionOrder>(),
                It.IsAny<CancellationToken>()),
            Times.Never
        );
    }

}
