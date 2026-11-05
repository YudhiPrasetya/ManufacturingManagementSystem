using Production.Application.Interfaces;
using Production.Domain.Entities;

namespace Production.Application.UseCases.ProductionOrders.GetProductionOrder;

public class GetProductionOrderHandler
{
    private readonly IProductionOrderRepository _repository;

    public GetProductionOrderHandler(IProductionOrderRepository repository)
    {
        _repository = repository;
    }

    public async Task<ProductionOrder?> Handle(GetProductionOrderQuery query, CancellationToken ct = default)
    {
        return await _repository.GetByOrderNumberAsync(query.OrderNumber, ct);
    }

}
