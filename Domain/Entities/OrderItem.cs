namespace GestaoPedidosAPI.Domain.Entities;

public class OrderItem
{
    public Guid Id { get; set; }
    public Guid OrderId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }

    private OrderItem() { }

    public OrderItem(Guid id, Guid orderId, string productName, int quantity, decimal unitPrice)
    {
        Id = id;
        OrderId = orderId;
        ProductName = productName;
        Quantity = quantity;
        UnitPrice = unitPrice;
    }
}