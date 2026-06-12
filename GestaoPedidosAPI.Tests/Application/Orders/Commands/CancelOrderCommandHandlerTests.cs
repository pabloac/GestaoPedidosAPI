using FluentAssertions;
using GestaoPedidosAPI.Application.Common.Interfaces;
using GestaoPedidosAPI.Application.Orders.Commands.CancelOrder;
using GestaoPedidosAPI.Domain.Entities;
using GestaoPedidosAPI.Domain.Enums;
using Moq;

namespace GestaoPedidosAPI.Tests.Application.Orders.Commands;

public class CancelOrderCommandHandlerTests
{
    private readonly Mock<IOrderRepository> _orderRepositoryMock = new();
    private readonly CancelOrderCommandHandler _handler;

    public CancelOrderCommandHandlerTests()
    {
        _handler = new CancelOrderCommandHandler(_orderRepositoryMock.Object);
    }

    [Fact]
    public async Task Handle_PedidoPending_CancelaERetornaResponse()
    {
        // Arrange
        var order = new Order(Guid.NewGuid(), Guid.NewGuid(), OrderStatus.Pending, DateTime.UtcNow);
        var command = new CancelOrderCommand(order.Id);

        _orderRepositoryMock
            .Setup(r => r.GetByIdAsync(order.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(order);

        _orderRepositoryMock
            .Setup(r => r.UpdateAsync(order, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result!.Status.Should().Be(OrderStatus.Cancelled.ToString());
        result.Descricao.Should().Be("Cancelado com sucesso!");
    }

    [Fact]
    public async Task Handle_PedidoNaoEncontrado_RetornaNull()
    {
        // Arrange
        var command = new CancelOrderCommand(Guid.NewGuid());

        _orderRepositoryMock
            .Setup(r => r.GetByIdAsync(command.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Order?)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task Handle_PedidoJaCancelado_LancaArgumentException()
    {
        // Arrange
        var order = new Order(Guid.NewGuid(), Guid.NewGuid(), OrderStatus.Cancelled, DateTime.UtcNow);
        var command = new CancelOrderCommand(order.Id);

        _orderRepositoryMock
            .Setup(r => r.GetByIdAsync(order.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(order);

        // Act
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>()
            .WithMessage("O pedido já está cancelado.");
    }

    [Fact]
    public async Task Handle_PedidoConfirmado_LancaArgumentExceptionComMotivo()
    {
        // Arrange
        var order = new Order(Guid.NewGuid(), Guid.NewGuid(), OrderStatus.Confirmed, DateTime.UtcNow);
        var command = new CancelOrderCommand(order.Id);

        _orderRepositoryMock
            .Setup(r => r.GetByIdAsync(order.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(order);

        // Act
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>()
            .WithMessage("*Confirmed*");
    }

    [Fact]
    public async Task Handle_PedidoPending_ChamaUpdateUmaVez()
    {
        // Arrange
        var order = new Order(Guid.NewGuid(), Guid.NewGuid(), OrderStatus.Pending, DateTime.UtcNow);
        var command = new CancelOrderCommand(order.Id);

        _orderRepositoryMock
            .Setup(r => r.GetByIdAsync(order.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(order);

        _orderRepositoryMock
            .Setup(r => r.UpdateAsync(order, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _orderRepositoryMock.Verify(
            r => r.UpdateAsync(It.IsAny<Order>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }
}
