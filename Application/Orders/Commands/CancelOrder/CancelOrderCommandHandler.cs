using GestaoPedidosAPI.Application.Common.Interfaces;
using MediatR;

namespace GestaoPedidosAPI.Application.Orders.Commands.CancelOrder
{
    public class CancelOrderCommandHandler : IRequestHandler<CancelOrderCommand, bool>
    {
        private readonly IOrderRepository _orderRepository;

        public CancelOrderCommandHandler(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        public async Task<bool> Handle(CancelOrderCommand request, CancellationToken cancellationToken)
        {
            var order = await _orderRepository.GetByIdAsync(request.Id, cancellationToken);

            if (order is null)
                throw new ArgumentException("Pedido não encontrado.");

            order.Cancel();

            return await _orderRepository.UpdateAsync(order, cancellationToken);
        }
    }
}
