using GestaoPedidosAPI.Application.Orders.Commands.CreateOrder;
using MediatR;

namespace GestaoPedidosAPI.Application.Orders.Commands;

public record CreateOrderCommand(
    Guid CustomerId,
    List<CreateOrderItemRequest> Items) : IRequest<CreateOrderResponse>;