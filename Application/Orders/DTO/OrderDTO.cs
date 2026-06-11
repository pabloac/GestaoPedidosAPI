using GestaoPedidosAPI.Application.Orders.DTO;

namespace GestaoPedidosAPI.Application.Orders.DTO;

public record OrderDTO(
    Guid Id,
    Guid CustomerId,
    string Status,
    DateTime CreatedAt,
    IReadOnlyList<OrderItemDTO> Items);