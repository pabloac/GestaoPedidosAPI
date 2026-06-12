using GestaoPedidosAPI.Application.Orders.DTO;

namespace GestaoPedidosAPI.Application.Orders.Commands.CancelOrder;

public record CancelOrderResponse(
    Guid Id,
    Guid CustomerId,
    string Status,
    string Descricao,
    IReadOnlyList<OrderItemDTO> Items);
