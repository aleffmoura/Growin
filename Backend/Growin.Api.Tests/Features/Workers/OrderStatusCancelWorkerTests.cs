namespace Growin.Api.Tests.Features.Workers;

using FluentAssertions;
using FunctionalConcepts.Results;
using Growin.Api.BackgroundServices;
using Growin.Domain.Enums;
using Growin.Domain.Features;
using Growin.Domain.Interfaces.WriteRepositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;

internal class OrderStatusCancelWorkerTests
{
    private readonly IConfiguration _mockConfigudation;
    private readonly Mock<ILogger<OrderStatusCancelWorker>> _mockLogger;
    private readonly Mock<IOrderWriteRepository> _mockOrderWriteRepository;
    private readonly Mock<IOrderReadRepository> _mockOrderReadRepository;
    private readonly Mock<IProductReadRepository> _mockProductReadRepository;
    private readonly Mock<IProductWriteRepository> _mockProductWriteRepository;
    private readonly OrderStatusCancelWorker _worker;

    public OrderStatusCancelWorkerTests()
    {
        var dictionary =
            new Dictionary<string, string?> {
                { "Workers:OrderStatusCancelWorker:NextIteratorInSeconds", "5" },
                { "Workers:OrderStatusCancelWorker:CancellOrderInSeconds", "5" }
            };

        _mockConfigudation = new ConfigurationBuilder()
            .AddInMemoryCollection(dictionary)
            .Build();
        _mockLogger = new();
        _mockOrderWriteRepository = new();
        _mockOrderReadRepository = new();
        _mockProductReadRepository = new();
        _mockProductWriteRepository = new();
        _worker = new(_mockLogger.Object, _mockOrderWriteRepository.Object, _mockOrderReadRepository.Object,
            _mockProductWriteRepository.Object, _mockProductReadRepository.Object, _mockConfigudation);
    }

    [Test]
    public async Task OrderStatusCancelWorkerTests_ExecuteAsync_ShouldBeOk()
    {
        // arrange
        var cancellationToken = new CancellationTokenSource();
        var orders = DbOrders();

        var dbProduct = new Product
        {
            Id = 1,
            CreatedAt = DateTime.UtcNow,
            Name = "",
            QuantityInStock = 5,
        };

        var expectedQuantityInStock = orders[1].Quantity + orders[2].Quantity + dbProduct.QuantityInStock;

        _mockOrderReadRepository.Setup(x => x.GetAllByStatus(It.IsAny<EOrderStatus>()))
            .Returns(orders.AsQueryable());

        _mockOrderWriteRepository.Setup(x => x.UpdateAsync(It.IsAny<Order>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success);

        _mockProductReadRepository.Setup(x => x.GetAsync(It.IsAny<long>(), It.IsAny<CancellationToken>()))
            .Returns(async (long id, CancellationToken _) => await Task.FromResult(dbProduct));

        _mockProductWriteRepository.Setup(x => x.UpdateAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()))
            .Returns(async (Product product, CancellationToken token) =>
            {
                dbProduct = product;
                return await Task.FromResult(Result.Success);
            });

        _ = Task.Delay(5000).ContinueWith(_ => cancellationToken.Cancel());

        // act
        var act = async () => await _worker.StartAsync(cancellationToken.Token);

        // assert
        await act.Should().NotThrowAsync();
        dbProduct.QuantityInStock.Should().Be(expectedQuantityInStock);

        _mockOrderReadRepository.Verify(x => x.GetAllByStatus(It.IsAny<EOrderStatus>()), times: Times.Once());
        _mockOrderWriteRepository.Verify(x => x.UpdateAsync(It.IsAny<Order>(), It.IsAny<CancellationToken>()), times: Times.Exactly(2));
        _mockProductReadRepository.Verify(x => x.GetAsync(It.IsAny<long>(), It.IsAny<CancellationToken>()), times: Times.Exactly(2));
        _mockProductWriteRepository.Verify(x => x.UpdateAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()), times: Times.Exactly(2));

        _mockOrderReadRepository.VerifyNoOtherCalls();
        _mockOrderWriteRepository.VerifyNoOtherCalls();
        _mockProductReadRepository.VerifyNoOtherCalls();
        _mockProductWriteRepository.VerifyNoOtherCalls();
    }

    private static List<Order> DbOrders()
        => [
            new ()
            {
                Id = 1,
                CreatedAt = DateTime.UtcNow.AddSeconds(-4),
                ProductId = 1,
                Quantity = 10,
                Status = EOrderStatus.Reserved,
            },
            new ()
            {
                Id = 1,
                CreatedAt = DateTime.UtcNow.AddSeconds(-6),
                ProductId = 1,
                Quantity = 5,
                Status = EOrderStatus.Reserved,
            },
            new ()
            {
                Id = 1,
                CreatedAt = DateTime.UtcNow.AddSeconds(-7),
                ProductId = 1,
                Quantity = 8,
                Status = EOrderStatus.Reserved,
            }
        ];
}
