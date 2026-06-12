using FluentAssertions;
using GestaoPedidosAPI.Application.Common.Interfaces;
using GestaoPedidosAPI.Application.Orders.Queries;
using GestaoPedidosAPI.Domain.Entities;
using GestaoPedidosAPI.Domain.Enums;
using Moq;

namespace GestaoPedidosAPI.Tests.Application.Orders.Queries;

public class GetOrdersQueryHandlerTests
{
    private readonly Mock<IOrderRepository> _orderRepositoryMock = new();
    private readonly GetOrdersQueryHandler _handler;

    public GetOrdersQueryHandlerTests()
    {
        _handler = new GetOrdersQueryHandler(_orderRepositoryMock.Object);
    }

    [Fact]
    public async Task Handle_RetornaPagedResultMapeado()
    {
        // Arrange
        var orders = new List<Order>
        {
            new(Guid.NewGuid(), Guid.NewGuid(), OrderStatus.Pending, DateTime.UtcNow),
            new(Guid.NewGuid(), Guid.NewGuid(), OrderStatus.Confirmed, DateTime.UtcNow)
        };

        var query = new GetOrdersQuery(Page: 1, PageSize: 10);

        _orderRepositoryMock
            .Setup(r => r.GetPagedAsync(1, 10, It.IsAny<CancellationToken>()))
            .ReturnsAsync((orders.AsReadOnly() as IReadOnlyList<Order>, 2));

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Items.Should().HaveCount(2);
        result.TotalCount.Should().Be(2);
        result.Page.Should().Be(1);
        result.PageSize.Should().Be(10);
    }

    [Fact]
    public async Task Handle_ListaVazia_RetornaPagedResultVazio()
    {
        // Arrange
        var query = new GetOrdersQuery(Page: 1, PageSize: 10);

        _orderRepositoryMock
            .Setup(r => r.GetPagedAsync(1, 10, It.IsAny<CancellationToken>()))
            .ReturnsAsync((new List<Order>().AsReadOnly() as IReadOnlyList<Order>, 0));

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Items.Should().BeEmpty();
        result.TotalCount.Should().Be(0);
    }

    [Fact]
    public async Task Handle_MapeiaStatusComoString()
    {
        // Arrange
        var orders = new List<Order>
        {
            new(Guid.NewGuid(), Guid.NewGuid(), OrderStatus.Pending, DateTime.UtcNow)
        };

        var query = new GetOrdersQuery(Page: 1, PageSize: 10);

        _orderRepositoryMock
            .Setup(r => r.GetPagedAsync(1, 10, It.IsAny<CancellationToken>()))
            .ReturnsAsync((orders.AsReadOnly() as IReadOnlyList<Order>, 1));

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Items[0].Status.Should().Be("Pending");
    }
}
