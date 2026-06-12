using GestaoPedidosAPI.Application.Orders.DTO;
using MediatR;

namespace GestaoPedidosAPI.Application.Orders.Queries;

public record GetOrderByIdQuery(Guid Id) : IRequest<OrderDTO?>;
