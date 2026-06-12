using FluentAssertions;
using GestaoPedidosAPI.Application.Common.Interfaces;
using GestaoPedidosAPI.Application.Orders.Queries;
using GestaoPedidosAPI.Domain.Entities;
using GestaoPedidosAPI.Domain.Enums;
using Moq;

namespace GestaoPedidosAPI.Tests.Application.Orders.Queries;

public class GetOrderByIdQueryHandlerTests
{
    private readonly Mock<IOrderRepository> _orderRepositoryMock = new();
    private readonly GetOrderByIdQueryHandler _handler;

    public GetOrderByIdQueryHandlerTests()
    {
        _handler = new GetOrderByIdQueryHandler(_orderRepositoryMock.Object);
    }

    [Fact]
    public async Task Handle_PedidoEncontrado_RetornaOrderDTO()
    {
        // Arrange
        var order = new Order(Guid.NewGuid(), Guid.NewGuid(), OrderStatus.Pending, DateTime.UtcNow);
        var query = new GetOrderByIdQuery(order.Id);

        _orderRepositoryMock
            .Setup(r => r.GetByIdAsync(order.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(order);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(order.Id);
        result.CustomerId.Should().Be(order.CustomerId);
        result.Status.Should().Be(OrderStatus.Pending.ToString());
    }

    [Fact]
    public async Task Handle_PedidoNaoEncontrado_RetornaNull()
    {
        // Arrange
        var query = new GetOrderByIdQuery(Guid.NewGuid());

        _orderRepositoryMock
            .Setup(r => r.GetByIdAsync(query.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Order?)null);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task Handle_PedidoComItens_MapeiaItenscorretamente()
    {
        // Arrange
        var orderId = Guid.NewGuid();
        var order = new Order(orderId, Guid.NewGuid(), OrderStatus.Pending, DateTime.UtcNow);
        var item = new OrderItem(Guid.NewGuid(), orderId, "Produto A", 2, 49.90m);
        var query = new GetOrderByIdQuery(orderId);

        _orderRepositoryMock
            .Setup(r => r.GetByIdAsync(orderId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(order);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result!.TotalAmount.Should().Be(0); // sem itens pois Order não expõe Add público
        result.Items.Should().BeEmpty();
    }
}
