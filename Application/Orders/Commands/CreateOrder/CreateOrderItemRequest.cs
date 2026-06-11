namespace GestaoPedidosAPI.Application.Orders.Commands.CreateOrder;

public record CreateOrderItemRequest(
    string ProductName,
    int Quantity,
    decimal UnitPrice);
