
using Microsoft.EntityFrameworkCore;
using Production.Application.Interfaces;
using Production.Domain.Entities;
using Production.Infrastructure.Persistence;

namespace Production.Infrastructure.Repositories;

public class ProductionOrderRepository : IProductionOrderRepository
{
    private readonly ProductionDbContext _context;

    public ProductionOrderRepository(ProductionDbContext context)
    {
        _context = context;
    }
    public async Task AddAsync(ProductionOrder productionOrder, CancellationToken ct = default)
    {
        await _context.ProductionOrders.AddAsync(productionOrder, ct);
        await _context.SaveChangesAsync(ct);
    }

    public async Task<ProductionOrder?> GetByOrderNumberAsync(string orderNumber, CancellationToken ct = default)
    {
        return await _context.ProductionOrders.FirstOrDefaultAsync(x => x.OrderNumber == orderNumber, ct);
    }

    public async Task<(IReadOnlyList<ProductionOrder> Items, int TotalCount)> GetPagedAsync(
        int page,
        int pageSize,
        string? status = null,
        CancellationToken ct = default)
    {
        var query = _context.ProductionOrders.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(status))
        {
            query = query.Where(x => x.Status == status);
        }

        var totalCount = await query.CountAsync(ct);

        var items = await query.OrderByDescending(x => x.CreatedAt)
                               .ThenBy(x => x.Id)
                               .Skip((page - 1) * pageSize)
                               .Take(pageSize)
                               .ToListAsync(ct);

        return (items, totalCount);

    }

    public async Task UpdateAsync(ProductionOrder productionOrder, CancellationToken ct = default)
    {
        _context.ProductionOrders.Update(productionOrder);

        await _context.SaveChangesAsync(ct);
    }
}
