﻿namespace Growin.ApplicationService.Tests.CreateOrderTests;

using AutoMapper;
using FluentAssertions;
using FunctionalConcepts.Results;
using Growin.ApplicationService.Features.Orders.Commands;
using Growin.ApplicationService.Features.Orders.CommandsHandlers;
using Growin.Domain.Features;
using Growin.Domain.Interfaces.WriteRepositories;
using MediatR;
using Moq;

public class CreateOrderHandlerTests
{
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<IOrderWriteRepository> _mockOrderWriteRepository;
    private readonly IRequestHandler<CreateOrderCommand, Result<long>> _handler;

    public CreateOrderHandlerTests()
    {
        _mockMapper = new();
        _mockOrderWriteRepository = new();
        _handler = new CreateOrderHandler(_mockOrderWriteRepository.Object, _mockMapper.Object);
    }

    [Test]
    public async Task CreateOrderHandlerTests_Handle_Create_ShouldBeSuccess()
    {
        // arrange
        var cancellationToken = new CancellationTokenSource().Token;

        var command = CreateCommand(1,100);
        var order = CreateOrder(command);

        _mockMapper.Setup(x => x.Map<Order>(command))
                   .Returns(order);

        var idCreated = 1;
        _mockOrderWriteRepository.Setup(x => x.AddAsync(order, cancellationToken))
            .ReturnsAsync(order with { Id = idCreated });

        // action
        var response = await _handler.Handle(command, cancellationToken);

        // verifies
        response.IsSuccess.Should().BeTrue();
        response.IsFail.Should().BeFalse();
        long id = 0;
        response.Then(idResult => id = idResult);
        id.Should().Be(idCreated);
    }

    CreateOrderCommand CreateCommand(long productId, ulong quantity) => new()
    {
        ProductId = productId,
        Quantity = quantity
    };

    Order CreateOrder(CreateOrderCommand createOrderCommand) => new()
    {
        Id = 0,
        ProductId = createOrderCommand.ProductId,
        Quantity = createOrderCommand.Quantity,
        Status = Domain.Enums.EOrderStatus.Reserved
    };
}
