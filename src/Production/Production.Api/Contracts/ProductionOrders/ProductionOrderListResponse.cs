namespace Production.Api.Contracts.ProductionOrders;

public record ProductionOrderListResponse(
    Guid Id, string OrderNumber,
    string ProductCode, string ProductName,
    int PlannedQuantity, int CompletedQuantity,
    DateTime CreatedAt, string Status
);

