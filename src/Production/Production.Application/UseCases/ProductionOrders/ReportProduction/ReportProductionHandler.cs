
using Production.Application.Interfaces;

namespace Production.Application.UseCases.ProductionOrders.ReportProduction;

public class ReportProductionHandler
{
    private readonly IProductionOrderRepository _repository;

    public ReportProductionHandler(IProductionOrderRepository repository)
    {
        _repository = repository;
    }

    public async Task Handle(
        ReportProductionCommand command,
        CancellationToken ct = default)
    {
        var productionOrder = await _repository.GetByOrderNumberAsync(command.OrderNumber, ct);

        if (productionOrder is null)
        {
            throw new KeyNotFoundException($"Production order '{command.OrderNumber}' not found.");
        }

        productionOrder.ReportProduction(command.Quantity);

        await _repository.UpdateAsync(productionOrder, ct);

    }
}
