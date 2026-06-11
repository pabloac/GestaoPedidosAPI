using GestaoPedidosAPI.Application.Common.Interfaces;
using GestaoPedidosAPI.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace GestaoPedidosAPI.Infrastructure.Persistence.Repositories;

public class OrderRepository : IOrderRepository
{
    private readonly AppDbContext _context;

    public OrderRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Order order, IEnumerable<OrderItem>? items = null, CancellationToken cancellationToken = default)
    {
        //Aqui meu objetivo é em deixar opcional adicionar ordem com ou sem itens, para implementar depois um endpoint pra adicionar itens a uma ordem já existente
        await _context.Orders.AddAsync(order, cancellationToken);

        if (items != null && items.Any())
            await _context.OrderItems.AddRangeAsync(items, cancellationToken);

        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<(IReadOnlyList<Order> Items, int TotalCount)> GetPagedAsync(
        int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var query = _context.Orders
            .Include(o => o.Items)
            .AsNoTracking();

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(o => o.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }
}