namespace GestaoPedidosAPI.Application.Orders.Commands.CreateOrder;

public record CreateOrderResponse(
    Guid Id,
    string Status,
    DateTime CreatedAt);
