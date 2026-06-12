using FluentAssertions;
using GestaoPedidosAPI.Application.Common.Interfaces;
using GestaoPedidosAPI.Application.Orders.Commands;
using GestaoPedidosAPI.Application.Orders.Commands.CreateOrder;
using GestaoPedidosAPI.Domain.Entities;
using GestaoPedidosAPI.Domain.Enums;
using Moq;

namespace GestaoPedidosAPI.Tests.Application.Orders.Commands;

public class CreateOrderCommandHandlerTests
{
    private readonly Mock<IOrderRepository> _orderRepositoryMock = new();
    private readonly CreateOrderCommandHandler _handler;

    public CreateOrderCommandHandlerTests()
    {
        _handler = new CreateOrderCommandHandler(_orderRepositoryMock.Object);
    }

    [Fact]
    public async Task Handle_CommandValido_RetornaResponseComStatusPending()
    {
        // Arrange
        var command = new CreateOrderCommand(
            Guid.NewGuid(),
            [new CreateOrderItemRequest("Produto A", 2, 49.90m)]);

        _orderRepositoryMock
            .Setup(r => r.AddAsync(It.IsAny<Order>(), It.IsAny<IEnumerable<OrderItem>>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().NotBeEmpty();
        result.Status.Should().Be(OrderStatus.Pending.ToString());
        result.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public async Task Handle_CommandValido_ChamaRepositorioUmaVez()
    {
        // Arrange
        var command = new CreateOrderCommand(
            Guid.NewGuid(),
            [new CreateOrderItemRequest("Produto A", 1, 10.00m)]);

        _orderRepositoryMock
            .Setup(r => r.AddAsync(It.IsAny<Order>(), It.IsAny<IEnumerable<OrderItem>>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _orderRepositoryMock.Verify(
            r => r.AddAsync(It.IsAny<Order>(), It.IsAny<IEnumerable<OrderItem>>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_MultiploItens_CriaItensCom_OrderIdCorreto()
    {
        // Arrange
        var customerId = Guid.NewGuid();
        Order? orderCapturado = null;
        IEnumerable<OrderItem>? itensCapturados = null;

        var command = new CreateOrderCommand(customerId,
        [
            new CreateOrderItemRequest("Produto A", 2, 49.90m),
            new CreateOrderItemRequest("Produto B", 1, 19.90m)
        ]);

        _orderRepositoryMock
            .Setup(r => r.AddAsync(It.IsAny<Order>(), It.IsAny<IEnumerable<OrderItem>>(), It.IsAny<CancellationToken>()))
            .Callback<Order, IEnumerable<OrderItem>?, CancellationToken>((o, i, _) =>
            {
                orderCapturado = o;
                itensCapturados = i;
            })
            .Returns(Task.CompletedTask);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        orderCapturado.Should().NotBeNull();
        orderCapturado!.CustomerId.Should().Be(customerId);
        itensCapturados.Should().HaveCount(2);
        itensCapturados!.Should().AllSatisfy(i => i.OrderId.Should().Be(orderCapturado.Id));
    }
}
