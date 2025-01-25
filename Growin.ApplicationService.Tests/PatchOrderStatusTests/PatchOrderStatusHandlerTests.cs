namespace Growin.ApplicationService.Tests.PatchOrderStatusTests;
using FluentAssertions;
using FunctionalConcepts.Errors;
using FunctionalConcepts.Options;
using FunctionalConcepts.Results;
using Growin.ApplicationService.Features.Orders.Commands;
using Growin.ApplicationService.Features.Orders.CommandsHandlers;
using Growin.Domain.Enums;
using Growin.Domain.Features;
using Growin.Domain.Interfaces.WriteRepositories;

using Moq;

public class PatchOrderStatusHandlerTests
{
    private readonly Mock<IOrderWriteRepository> _mockOrderWriteRepository;
    private readonly Mock<IOrderReadRepository> _mockOrderReadRepository;
    private readonly PatchOrderStatusHandler _handler;

    public PatchOrderStatusHandlerTests()
    {
        _mockOrderWriteRepository = new();
        _mockOrderReadRepository = new();
        _handler = new(_mockOrderWriteRepository.Object, _mockOrderReadRepository.Object);
    }

    [Test]
    public async Task PatchOrderStatusHandlerTests_Handle_ShouldBeOk()
    {
        // Arrange
        var cancellationToken = new CancellationTokenSource().Token;
        var command = new PatchOrderStatusCommand
        {
            OrderId = 1
        };
        var order = ReservedOrder();

        _mockOrderWriteRepository.Setup(x => x.UpdateAsync(It.Is<Order>(x => x.Status == EOrderStatus.Finished &&
                                                                              x.UpdatedAt != null),
                                                            cancellationToken))
            .ReturnsAsync(Result.Success);

        _mockOrderReadRepository.Setup(x => x.GetAsync(command.OrderId, cancellationToken))
            .ReturnsAsync(order);

        // Act
        var result = await _handler.Handle(command, cancellationToken);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.IsFail.Should().BeFalse();

        _mockOrderWriteRepository.Verify(x => x.UpdateAsync(It.Is<Order>(x => x.Status == EOrderStatus.Finished &&
                                                                              x.UpdatedAt != null),
                                                cancellationToken));
        _mockOrderReadRepository.Verify(x => x.GetAsync(command.OrderId, cancellationToken));
        _mockOrderWriteRepository.VerifyNoOtherCalls();
        _mockOrderReadRepository.VerifyNoOtherCalls();
    }

    [Test]
    public async Task PatchOrderStatusHandlerTests_Handle_OrderNotFound_ShouldBeError()
    {
        // Arrange
        var cancellationToken = new CancellationTokenSource().Token;
        var command = new PatchOrderStatusCommand
        {
            OrderId = 1
        };

        _mockOrderReadRepository.Setup(x => x.GetAsync(command.OrderId, cancellationToken))
            .ReturnsAsync(NoneType.Value);

        // Act
        var result = await _handler.Handle(command, cancellationToken);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.IsFail.Should().BeTrue();
        BaseError? baseError = null;
        result.Else(err => baseError = err);
        baseError.Should().NotBeNull();
        baseError.Should()
            .BeOfType<NotFoundError>()
            .Subject.Message
            .Should()
            .Be("Order not found");

        _mockOrderReadRepository.Verify(x => x.GetAsync(command.OrderId, cancellationToken));
        _mockOrderWriteRepository.VerifyNoOtherCalls();
        _mockOrderReadRepository.VerifyNoOtherCalls();
    }

    [Test]
    public async Task PatchOrderStatusHandlerTests_Handle_OrderClosed_ShouldBeError()
    {
        // Arrange
        var cancellationToken = new CancellationTokenSource().Token;
        var command = new PatchOrderStatusCommand
        {
            OrderId = 1
        };
        var orderClosed = OrderClosed();

        _mockOrderReadRepository.Setup(x => x.GetAsync(command.OrderId, cancellationToken))
            .ReturnsAsync(orderClosed);

        // Act
        var result = await _handler.Handle(command, cancellationToken);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.IsFail.Should().BeTrue();
        BaseError? baseError = null;
        result.Else(err => baseError = err);
        baseError.Should().NotBeNull();
        baseError.Should()
            .BeOfType<InvalidObjectError>()
            .Subject.Message
            .Should()
            .Be($"Order with status: {orderClosed.Status}");

        _mockOrderReadRepository.Verify(x => x.GetAsync(command.OrderId, cancellationToken));
        _mockOrderWriteRepository.VerifyNoOtherCalls();
        _mockOrderReadRepository.VerifyNoOtherCalls();
    }

    [Test]
    public async Task PatchOrderStatusHandlerTests_Handle_UpdateAsync_ThrowsExn_ButReturnError()
    {
        // Arrange
        var cancellationToken = new CancellationTokenSource().Token;
        var command = new PatchOrderStatusCommand
        {
            OrderId = 1
        };
        var order = ReservedOrder();

        var exn = new InvalidDataException();
        _mockOrderWriteRepository.Setup(x => x.UpdateAsync(It.Is<Order>(x => x.Status == EOrderStatus.Finished &&
                                                                              x.UpdatedAt != null),
                                                            cancellationToken))
            .Throws(exn);

        _mockOrderReadRepository.Setup(x => x.GetAsync(command.OrderId, cancellationToken))
            .ReturnsAsync(order);

        // Act
        var result = await _handler.Handle(command, cancellationToken);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.IsFail.Should().BeTrue();

        BaseError? baseError = null;
        result.Else(err => baseError = err);
        baseError.Should().NotBeNull();
        baseError.Should()
            .BeOfType<UnhandledError>()
            .Subject.Message
            .Should()
            .Be("Handled exception on create order");

        _mockOrderWriteRepository.Verify(x => x.UpdateAsync(It.Is<Order>(x => x.Status == EOrderStatus.Finished &&
                                                                              x.UpdatedAt != null),
                                                cancellationToken));
        _mockOrderReadRepository.Verify(x => x.GetAsync(command.OrderId, cancellationToken));
        _mockOrderWriteRepository.VerifyNoOtherCalls();
        _mockOrderReadRepository.VerifyNoOtherCalls();
    }

    [Test]
    public async Task PatchOrderStatusHandlerTests_Handle_GetAsync_ThrowsExn_ButReturnError()
    {
        // Arrange
        var cancellationToken = new CancellationTokenSource().Token;
        var command = new PatchOrderStatusCommand
        {
            OrderId = 1
        };

        var exn = new InvalidDataException();
        _mockOrderReadRepository.Setup(x => x.GetAsync(command.OrderId, cancellationToken))
            .Throws(exn);

        // Act
        var result = await _handler.Handle(command, cancellationToken);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.IsFail.Should().BeTrue();

        BaseError? baseError = null;
        result.Else(err => baseError = err);
        baseError.Should().NotBeNull();
        baseError.Should()
            .BeOfType<UnhandledError>()
            .Subject.Message
            .Should()
            .Be("Handled exception on create order");

        _mockOrderReadRepository.Verify(x => x.GetAsync(command.OrderId, cancellationToken));
        _mockOrderWriteRepository.VerifyNoOtherCalls();
        _mockOrderReadRepository.VerifyNoOtherCalls();
    }

    #region private methods
    Order ReservedOrder() => new()
    {
        Id = 1,
        ProductId = 1,
        Quantity = 100,
        Status = EOrderStatus.Reserved,
        CreatedAt = DateTime.UtcNow,
    };
    Order OrderClosed() => new()
    {
        Id = 1,
        ProductId = 1,
        Quantity = 100,
        Status = EOrderStatus.Closed,
        CreatedAt = DateTime.UtcNow,
    };
    #endregion
}
