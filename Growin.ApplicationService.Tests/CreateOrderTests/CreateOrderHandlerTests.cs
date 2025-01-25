namespace Growin.ApplicationService.Tests.CreateOrderTests;

using AutoMapper;
using FluentAssertions;
using FunctionalConcepts;
using FunctionalConcepts.Errors;
using FunctionalConcepts.Options;
using FunctionalConcepts.Results;
using Growin.ApplicationService.Features.Orders.Commands;
using Growin.ApplicationService.Features.Orders.CommandsHandlers;
using Growin.Domain.Features;
using Growin.Domain.Interfaces.WriteRepositories;
using Moq;

public class CreateOrderHandlerTests
{
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<IOrderWriteRepository> _mockOrderWriteRepository;
    private readonly Mock<IProductReadRepository> _mockProductReadRepository;
    private readonly Mock<IProductWriteRepository> _mockProductWriteRepository;
    private readonly CreateOrderHandler _handler;

    public CreateOrderHandlerTests()
    {
        _mockMapper = new();
        _mockOrderWriteRepository = new();
        _mockProductReadRepository = new();
        _mockProductWriteRepository = new();
        _handler = new(_mockOrderWriteRepository.Object, _mockProductReadRepository.Object,
            _mockProductWriteRepository.Object, _mockMapper.Object);
    }

    [Test]
    public async Task CreateOrderHandlerTests_Handle_Create_ShouldBeSuccess()
    {
        // arrange
        var cancellationToken = new CancellationTokenSource().Token;

        var command = CreateCommand(1, 100);
        var order = CreateOrder(command);
        var product = CreateProduct(command.ProductId);

        _mockProductReadRepository.Setup(x => x.GetAsync(command.ProductId, cancellationToken))
            .ReturnsAsync(product);

        _mockMapper.Setup(x => x.Map<Order>(command))
                   .Returns(order);

        var idCreated = 1;
        _mockOrderWriteRepository.Setup(x => x.AddAsync(order, cancellationToken))
            .ReturnsAsync(order with { Id = idCreated });

        var newQuantity = product.QuantityInStock - command.Quantity;
        _mockProductWriteRepository.Setup(x=> x.UpdateAsync(It.Is<Product>(x => x.QuantityInStock == newQuantity), cancellationToken))
            .ReturnsAsync(Result.Success);

        // action
        var response = await _handler.Handle(command, cancellationToken);

        // verifies
        response.IsSuccess.Should().BeTrue();
        response.IsFail.Should().BeFalse();
        long id = 0;
        response.Then(idResult => id = idResult);
        id.Should().Be(idCreated);
        _mockProductReadRepository.Verify(x => x.GetAsync(command.ProductId, cancellationToken));
        _mockMapper.Verify(x => x.Map<Order>(command));
        _mockOrderWriteRepository.Verify(x => x.AddAsync(order, cancellationToken));
        _mockProductWriteRepository.Verify(x => x.UpdateAsync(It.Is<Product>(x => x.QuantityInStock == newQuantity), cancellationToken));
        _mockProductReadRepository.VerifyNoOtherCalls();
        _mockMapper.VerifyNoOtherCalls();
        _mockOrderWriteRepository.VerifyNoOtherCalls();
        _mockProductWriteRepository.VerifyNoOtherCalls();
    }

    [Test]
    public async Task CreateOrderHandlerTests_Handle_Create_AddAsyncThrowsExnBut_ShouldBeError()
    {
        // Arrange
        var cancellationToken = new CancellationTokenSource().Token;

        var command = CreateCommand(1, 100);
        var order = CreateOrder(command);
        var product = CreateProduct(command.ProductId);

        _mockProductReadRepository.Setup(x => x.GetAsync(command.ProductId, cancellationToken))
            .ReturnsAsync(product);

        _mockMapper.Setup(x => x.Map<Order>(command))
                   .Returns(order);

        var exn = new InvalidDataException();
        _mockOrderWriteRepository.Setup(x => x.AddAsync(order, cancellationToken))
            .Throws(exn);

        // Act
        var result = await _handler.Handle(command, cancellationToken);

        // Asserts
        result.IsSuccess.Should().BeFalse();
        result.IsFail.Should().BeTrue();
        _mockMapper.Verify(x => x.Map<Order>(command));
        _mockMapper.VerifyNoOtherCalls();
        _mockOrderWriteRepository.Verify(x => x.AddAsync(order, cancellationToken));
        _mockOrderWriteRepository.VerifyNoOtherCalls();
        BaseError? baseError = null;
        result.Else(err => baseError = err);
        baseError.Should().NotBeNull();
        baseError.Should()
            .BeOfType<UnhandledError>()
            .Subject.Message
            .Should()
            .Be("Handled exception on create order");
        _mockProductReadRepository.Verify(x => x.GetAsync(command.ProductId, cancellationToken));
        _mockMapper.Verify(x => x.Map<Order>(command));
        _mockOrderWriteRepository.Verify(x => x.AddAsync(order, cancellationToken));
        _mockProductReadRepository.VerifyNoOtherCalls();
        _mockMapper.VerifyNoOtherCalls();
        _mockOrderWriteRepository.VerifyNoOtherCalls();
        _mockProductWriteRepository.VerifyNoOtherCalls();
    }

    [Test]
    public async Task CreateOrderHandlerTests_Handle_Create_ProductOutOfStock_ShouldBeError()
    {
        // Arrange
        var cancellationToken = new CancellationTokenSource().Token;

        var command = CreateCommand(1, 100);
        var order = CreateOrder(command);
        var product = CreateProduct(command.ProductId, 99);

        _mockProductReadRepository.Setup(x => x.GetAsync(command.ProductId, cancellationToken))
            .ReturnsAsync(product);

        // Act
        var result = await _handler.Handle(command, cancellationToken);

        // Asserts
        result.IsSuccess.Should().BeFalse();
        result.IsFail.Should().BeTrue();

        BaseError? baseError = null;
        result.Else(err => baseError = err);
        baseError.Should().NotBeNull();
        baseError.Should()
            .BeOfType<InvalidObjectError>()
            .Subject.Message
            .Should()
            .Be("ProductOutOfStock");

        _mockProductReadRepository.Verify(x => x.GetAsync(command.ProductId, cancellationToken));
        _mockProductReadRepository.VerifyNoOtherCalls();
        _mockMapper.VerifyNoOtherCalls();
        _mockOrderWriteRepository.VerifyNoOtherCalls();
        _mockProductWriteRepository.VerifyNoOtherCalls();
    }

    [Test]
    public async Task CreateOrderHandlerTests_Handle_Create_ProductNotFound_ShouldBeError()
    {
        // Arrange
        var cancellationToken = new CancellationTokenSource().Token;

        var command = CreateCommand(1, 100);
        var order = CreateOrder(command);

        _mockProductReadRepository.Setup(x => x.GetAsync(command.ProductId, cancellationToken))
            .ReturnsAsync(NoneType.Value);

        // Act
        var result = await _handler.Handle(command, cancellationToken);

        // Asserts
        result.IsSuccess.Should().BeFalse();
        result.IsFail.Should().BeTrue();

        BaseError? baseError = null;
        result.Else(err => baseError = err);
        baseError.Should().NotBeNull();
        baseError.Should()
            .BeOfType<NotFoundError>()
            .Subject.Message
            .Should()
            .Be("ProductNotFound");

        _mockProductReadRepository.Verify(x => x.GetAsync(command.ProductId, cancellationToken));
        _mockProductReadRepository.VerifyNoOtherCalls();
        _mockMapper.VerifyNoOtherCalls();
        _mockOrderWriteRepository.VerifyNoOtherCalls();
        _mockProductWriteRepository.VerifyNoOtherCalls();
    }

    #region Private mathods
    CreateOrderCommand CreateCommand(long productId, long quantity) => new()
    {
        ProductId = productId,
        Quantity = quantity
    };

    Order CreateOrder(CreateOrderCommand createOrderCommand) => new()
    {
        Id = 0,
        ProductId = createOrderCommand.ProductId,
        Quantity = (ulong)createOrderCommand.Quantity,
        Status = Domain.Enums.EOrderStatus.Reserved,
        CreatedAt = DateTime.UtcNow,
    };

    Product CreateProduct(long productId, long quantity = 100) => new()
    {
        Id = productId,
        Name = "Test",
        QuantityInStock = quantity,
        CreatedAt = DateTime.UtcNow,
    };
    #endregion
}
