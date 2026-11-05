
using Production.Domain.Entities;

namespace Production.Application.Interfaces;

public interface IProductionOrderRepository
{
    Task AddAsync(ProductionOrder productionOrder, CancellationToken ct = default);
    Task UpdateAsync(ProductionOrder productionOrder, CancellationToken ct = default);
    Task<ProductionOrder?> GetByOrderNumberAsync(string orderNumber, CancellationToken ct = default);
    Task<(IReadOnlyList<ProductionOrder> Items, int TotalCount)> GetPagedAsync(
        int page,
        int pageSize,
        string? status = null,
        CancellationToken ct = default
    );
}
