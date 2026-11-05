namespace Production.Application.UseCases.ProductionOrders.GetProductionOrders;

public record GetProductionOrdersQuery(
    int Page = 1,
    int PageSize = 10,
    string? Status = null
);
