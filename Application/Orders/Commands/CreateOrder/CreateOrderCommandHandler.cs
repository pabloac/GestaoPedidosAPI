using GestaoPedidosAPI.Application.Common.Interfaces;
using GestaoPedidosAPI.Application.Orders.Commands.CreateOrder;
using GestaoPedidosAPI.Domain.Entities;
using GestaoPedidosAPI.Domain.Enums;
using MediatR;

namespace GestaoPedidosAPI.Application.Orders.Commands;

public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, CreateOrderResponse>
{
    private readonly IOrderRepository _orderRepository;

    public CreateOrderCommandHandler(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public async Task<CreateOrderResponse> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        var order = new Order(
            id: Guid.NewGuid(),
            customerId: request.CustomerId,
            status: OrderStatus.Pending,
            createdAt: DateTime.UtcNow);

        var items = request.Items.Select(i => new OrderItem(
            id: Guid.NewGuid(),
            orderId: order.Id,
            productName: i.ProductName,
            quantity: i.Quantity,
            unitPrice: i.UnitPrice)).ToList();

        await _orderRepository.AddAsync(order, items, cancellationToken);

        return new CreateOrderResponse(order.Id, order.Status.ToString(), order.CreatedAt);
    }
}