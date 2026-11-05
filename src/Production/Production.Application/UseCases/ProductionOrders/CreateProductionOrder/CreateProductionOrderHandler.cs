using System;
using Production.Application.Interfaces;
using Production.Domain.Entities;

namespace Production.Application.UseCases.ProductionOrders.CreateProductionOrder;

public class CreateProductionOrderHandler
{
    private readonly IProductionOrderRepository _repository;

    public CreateProductionOrderHandler(
        IProductionOrderRepository repository)
    {
        _repository = repository;
    }

    public async Task<Guid> Handle(
        CreateProductionOrderCommand command,
        CancellationToken cancellationToken = default)
    {
        var existingOrder =
            await _repository.GetByOrderNumberAsync(
                command.OrderNumber,
                cancellationToken);

        if (existingOrder is not null)
        {
            throw new InvalidOperationException(
                "Production order number already exists.");
        }

        var productionOrder = new ProductionOrder(
            command.OrderNumber,
            command.ProductCode,
            command.ProductName,
            command.PlannedQuantity
        );

        await _repository.AddAsync(
            productionOrder,
            cancellationToken);

        return productionOrder.Id;
    }
}
