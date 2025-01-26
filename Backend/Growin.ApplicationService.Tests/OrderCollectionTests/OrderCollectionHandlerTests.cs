namespace Growin.ApplicationService.Tests.OrderCollectionTests;

using FluentAssertions;
using FunctionalConcepts.Errors;
using Growin.ApplicationService.Features.Orders.Queries;
using Growin.ApplicationService.Features.Orders.QueryHandlers;
using Growin.Domain.Enums;
using Growin.Domain.Features;
using Growin.Domain.Interfaces.WriteRepositories;

using Moq;

public class OrderCollectionHandlerTests
{
    private readonly Mock<IOrderReadRepository> _mockReadRepository;
    private readonly OrderCollectionHandler _handler;

    public OrderCollectionHandlerTests()
    {
        _mockReadRepository = new();
        _handler = new(_mockReadRepository.Object);
    }

    [Test]
    public async Task OrderCollectionHandlerTests_Handle_ShouldBeOk()
    {
        // Arrange
        var cancellationToken = new CancellationTokenSource().Token;
        var query = new OrderCollectionQuery();
        List<Order> ordersDb =
        [
            CreateOrder(),CreateOrder(),CreateOrder(),
            CreateOrder(),CreateOrder(),CreateOrder()
        ];

        _mockReadRepository.Setup(x => x.GetAll())
            .Returns(ordersDb.AsQueryable());

        // Act
        var result = await _handler.Handle(query, cancellationToken);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.IsFail.Should().BeFalse();
        IQueryable<Order>? orders = null;

        result.Then(ords => orders = ords);
        orders.Should().NotBeNullOrEmpty();
        orders.Should().HaveCount(ordersDb.Count);
        orders.Should().BeEquivalentTo(ordersDb);

        _mockReadRepository.Verify(x => x.GetAll());
        _mockReadRepository.VerifyNoOtherCalls();
    }

    [Test]
    public async Task OrderCollectionHandlerTests_Handle_GetAllThrowsExnBut_ShouldBeError()
    {
        // Arrange
        var cancellationToken = new CancellationTokenSource().Token;
        var query = new OrderCollectionQuery();
        List<Order> ordersDb =
        [
            CreateOrder(),CreateOrder(),CreateOrder(),
            CreateOrder(),CreateOrder(),CreateOrder()
        ];

        var exn = new InvalidDataException();
        _mockReadRepository.Setup(x => x.GetAll())
            .Throws(exn);

        // Act
        var result = await _handler.Handle(query, cancellationToken);

        // Asserts
        result.IsSuccess.Should().BeFalse();
        result.IsFail.Should().BeTrue();
        BaseError? baseError = null;
        result.Else(err => baseError = err);
        baseError.Should().NotBeNull();
        baseError.Should()
            .BeOfType<UnhandledError>()
            .Subject.Message
            .Should()
            .Be("Handled exception on get all orders");

        _mockReadRepository.Verify(x => x.GetAll());
        _mockReadRepository.VerifyNoOtherCalls();
    }

    Order CreateOrder()
        => new()
        {
            Id = 1,
            ProductId = 1,
            Quantity = 10,
            CreatedAt = DateTime.UtcNow,
            Status = EOrderStatus.Reserved
        };
}
