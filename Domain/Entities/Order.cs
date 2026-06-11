using GestaoPedidosAPI.Domain.Enums;

namespace GestaoPedidosAPI.Domain.Entities;

public class Order
{
    public Guid Id { get; set; }
    public Guid CustomerId { get; set; }
    public OrderStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public IReadOnlyList<OrderItem> Items { get; set; } = [];

    private Order() { }

    public Order(Guid id, Guid customerId, OrderStatus status, DateTime createdAt)
    {
        Id = id;
        CustomerId = customerId;
        Status = status;
        CreatedAt = createdAt;
    }
}