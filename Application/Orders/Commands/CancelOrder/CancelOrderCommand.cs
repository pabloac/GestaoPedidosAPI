using MediatR;

namespace GestaoPedidosAPI.Application.Orders.Commands.CancelOrder;

public record CancelOrderCommand(
    Guid Id) : IRequest<CancelOrderResponse?>;