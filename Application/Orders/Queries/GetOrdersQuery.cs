using GestaoPedidosAPI.Application.Orders.DTO;
using MediatR;

namespace GestaoPedidosAPI.Application.Orders.Queries;

public record GetOrdersQuery(int Page, int PageSize) : IRequest<PagedResult<OrderDTO>>;
