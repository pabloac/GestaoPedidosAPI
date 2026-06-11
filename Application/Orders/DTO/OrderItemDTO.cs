namespace GestaoPedidosAPI.Application.Orders.DTO;

public record OrderItemDTO(
    Guid Id,
    string ProductName,
    int Quantity,
    decimal UnitPrice);