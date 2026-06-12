using GestaoPedidosAPI.Application.Common.Interfaces;
using GestaoPedidosAPI.Application.Orders.DTO;
using MediatR;

namespace GestaoPedidosAPI.Application.Orders.Commands.CancelOrder;

public class CancelOrderCommandHandler : IRequestHandler<CancelOrderCommand, CancelOrderResponse?>
{
    private readonly IOrderRepository _orderRepository;

    public CancelOrderCommandHandler(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public async Task<CancelOrderResponse?> Handle(CancelOrderCommand request, CancellationToken cancellationToken)
    {
        var order = await _orderRepository.GetByIdAsync(request.Id, cancellationToken);

        if (order is null)
            return null;

        order.Cancel();

        await _orderRepository.UpdateAsync(order, cancellationToken);

        return new CancelOrderResponse(
            order.Id,
            order.CustomerId,
            order.Status.ToString(),
            "Cancelado com sucesso!",
            order.Items.Select(i => new OrderItemDTO(i.Id, i.ProductName, i.Quantity, i.UnitPrice)).ToList());
    }
}
