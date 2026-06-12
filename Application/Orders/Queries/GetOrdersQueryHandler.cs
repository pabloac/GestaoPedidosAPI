using GestaoPedidosAPI.Application.Common.Interfaces;
using GestaoPedidosAPI.Application.Orders.DTO;
using MediatR;

namespace GestaoPedidosAPI.Application.Orders.Queries;

public class GetOrdersQueryHandler : IRequestHandler<GetOrdersQuery, PagedResult<OrderDTO>>
{
    private readonly IOrderRepository _orderRepository;

    public GetOrdersQueryHandler(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public async Task<PagedResult<OrderDTO>> Handle(GetOrdersQuery request, CancellationToken cancellationToken)
    {
        var (items, totalCount) = await _orderRepository.GetPagedAsync(
            request.Page, request.PageSize, cancellationToken);

        var dtos = items.Select(o => new OrderDTO(
            o.Id,
            o.CustomerId,
            o.Status.ToString(),
            o.CreatedAt,
            o.TotalAmount,
            o.Items.Select(i => new OrderItemDTO(i.Id, i.ProductName, i.Quantity, i.UnitPrice))
                   .ToList()
        )).ToList();

        return new PagedResult<OrderDTO>(dtos, request.Page, request.PageSize, totalCount);
    }
}
