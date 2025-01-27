namespace Growin.Api.BackgroundServices;

using Growin.Domain.Interfaces.WriteRepositories;
using Microsoft.Extensions.Logging;

public class OrderStatusCancelWorker(
    ILogger<OrderStatusCancelWorker> logger,
    IOrderWriteRepository repositoryWrite,
    IOrderReadRepository repositoryRead,
    IProductWriteRepository repositoryProductWrite,
    IProductReadRepository repositoryProductRead,
    IConfiguration configuration) : BackgroundService
{
    private readonly ILogger<OrderStatusCancelWorker> _logger = logger;

    private readonly IOrderWriteRepository _orderWriteRepository = repositoryWrite;
    private readonly IOrderReadRepository _orderReadRepository = repositoryRead;

    private readonly IProductWriteRepository _productWriteRepository = repositoryProductWrite;
    private readonly IProductReadRepository _productReadRepository = repositoryProductRead;

    private readonly IConfiguration _configuration = configuration;
    private readonly int _nextIterator = configuration.GetValue<int>("Workers:OrderStatusCancelWorker:NextIteratorInSeconds");
    private readonly int _cancellIn = configuration.GetValue<int>("Workers:OrderStatusCancelWorker:CancellOrderInSeconds");
    private async Task Invoke()
    {
        var orders = _orderReadRepository.GetAllByStatus(Domain.Enums.EOrderStatus.Reserved).ToArray();

        foreach (var order in orders)
        {
            try
            {
                var orderDate = order.CreatedAt;
                var currentDate = DateTime.UtcNow;
                var minutes = ( currentDate - orderDate ).TotalSeconds;

                if (minutes > _cancellIn)
                {
                    _ = await _orderWriteRepository.UpdateAsync(order with
                    {
                        Status = Domain.Enums.EOrderStatus.Canceled,
                        UpdatedAt = DateTime.UtcNow
                    }, cancellationToken: CancellationToken.None);

                    var productOpt = await _productReadRepository
                         .GetAsync(order.ProductId, CancellationToken.None);

                    var msg = await productOpt.MatchAsync(async product =>
                    {
                        _ = await _productWriteRepository.UpdateAsync(product with
                        {
                            UpdatedAt = DateTime.UtcNow,
                            QuantityInStock = product.QuantityInStock + order.Quantity
                        }, CancellationToken.None);
                        return "Order {orderId} was closed and value as re-stock";
                    }, () => $"Product with id: {order.ProductId} not exists on db");

                    _logger.LogInformation(msg);
                }
            }
            catch(Exception ex)
            {
                _logger.LogError("Exception while class:{className} message: {errorMessage}", nameof(OrderStatusCancelWorker), ex.Message);
            }
        }
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                _logger.LogInformation($"Executing task: {nameof(OrderStatusCancelWorker)}...");

                await Invoke();

                _logger.LogInformation($"End task: {nameof(OrderStatusCancelWorker)}...");

                await Task.Delay(_nextIterator * 1000, stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError("Exception while class:{className} message: {errorMessage}", nameof(OrderStatusCancelWorker), ex.Message);
            }
        }
    }
}