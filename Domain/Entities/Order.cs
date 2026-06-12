using GestaoPedidosAPI.Domain.Enums;

namespace GestaoPedidosAPI.Domain.Entities;

public class Order
{
    public Guid Id { get; set; }
    public Guid CustomerId { get; set; }
    public OrderStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }

    private readonly List<OrderItem> _items = [];
    public IReadOnlyList<OrderItem> Items => _items;

    private Order() { }

    public Order(Guid id, Guid customerId, OrderStatus status, DateTime createdAt)
    {
        Id = id;
        CustomerId = customerId;
        Status = status;
        CreatedAt = createdAt;
    }

    public void Cancel()
    {
        if (Status == OrderStatus.Cancelled)
            throw new ArgumentException("O pedido já está cancelado.");

        Status = OrderStatus.Cancelled;
    }
}