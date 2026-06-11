using GestaoPedidosAPI.Domain.Entities;

namespace GestaoPedidosAPI.Application.Common.Interfaces;

public interface IOrderRepository
{
    Task<(IReadOnlyList<Order> Items, int TotalCount)> GetPagedAsync(
        int page,
        int pageSize,
        CancellationToken cancellationToken = default);
}