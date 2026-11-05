using System;
using Moq;
using Production.Application.Interfaces;
using Production.Application.UseCases.ProductionOrders.GetProductionOrders;

namespace Production.Tests;

public class GetProductionOrdersHandlerTests
{
    [Fact]
    public async Task Handle_ShouldReturnPagedOrders_WhenQueryIsValid()
    {
        // Arrange
        var repositoryMock = new Mock<IProductionOrderRepository>();

        repositoryMock
            .Setup(r => r.GetPagedAsync(
                1,
                10,
                null,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((
                (IReadOnlyList<Production.Domain.Entities.ProductionOrder>)[],
                0
            ));

        var handler = new GetProductionOrdersHandler(repositoryMock.Object);

        var query = new GetProductionOrdersQuery(1, 10);

        // Act
        var result = await handler.Handle(query);

        // Assert
        Assert.Empty(result.Items);
        Assert.Equal(0, result.TotalCount);

        repositoryMock.Verify(
            r => r.GetPagedAsync(
                1,
                10,
                null,
                It.IsAny<CancellationToken>()),
            Times.Once
        );

    }

    [Theory]
    [InlineData(0, 10)]
    [InlineData(-1, 10)]
    [InlineData(1, 0)]
    [InlineData(1, 101)]
    public async Task Handle_ShouldThrowArgumentException_WhenPaginationIsInvalid(
        int page,
        int pageSize)
    {
        // Arrange
        var repositoryMock = new Mock<IProductionOrderRepository>();
        var handler = new GetProductionOrdersHandler(repositoryMock.Object);

        var query = new GetProductionOrdersQuery(page, pageSize);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(
            () => handler.Handle(query)
        );

        repositoryMock.Verify(
            r => r.GetPagedAsync(
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<string?>(),
                It.IsAny<CancellationToken>()),
            Times.Never
        );
    }

}
