using Production.Application.Interfaces;
using Production.Domain.Entities;

namespace Production.Application.UseCases.ProductionOrders.GetProductionOrders;

public class GetProductionOrdersHandler
{
    private readonly IProductionOrderRepository _repository;

    public GetProductionOrdersHandler(
        IProductionOrderRepository repository)
    {
        _repository = repository;
    }

    public async Task<(IReadOnlyList<ProductionOrder> Items, int TotalCount)> Handle(
        GetProductionOrdersQuery query,
        CancellationToken ct = default)
    {
        if (query.Page < 1)
        {
            throw new ArgumentException("Page must be greater than or equal to 1.");
        }

        if (query.PageSize < 1 || query.PageSize > 100)
        {
            throw new ArgumentException("Page size must be between 1 and 100.");
        }

        return await _repository.GetPagedAsync(
            query.Page, query.PageSize,
            query.Status, ct);
    }
}
