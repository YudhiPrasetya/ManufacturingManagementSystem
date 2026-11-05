using System;

namespace Production.Api.Contracts.ProductionOrders;

public record CreateProductionOrderRequest(
    string OrderNumber, string ProductCode,
    string ProductName, int PlannedQuantity
);

