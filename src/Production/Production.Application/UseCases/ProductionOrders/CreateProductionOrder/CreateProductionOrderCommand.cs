namespace Production.Application.UseCases.ProductionOrders.CreateProductionOrder;

public record CreateProductionOrderCommand(
    string OrderNumber, string ProductCode,
    string ProductName, int PlannedQuantity
);

