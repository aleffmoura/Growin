namespace Growin.Api.BackgroundServices;

using Growin.Domain.Interfaces.WriteRepositories;

public class OrderStatusCancelWorker(
    ILogger<OrderStatusCancelWorker> logger,
    IOrderWriteRepository repositoryWrite,
    IOrderReadRepository repositoryRead,
    IConfiguration configuration) : BackgroundService
{
    private readonly ILogger<OrderStatusCancelWorker> _logger = logger;
    private readonly IOrderWriteRepository _orderWriteRepository = repositoryWrite;
    private readonly IOrderReadRepository _orderReadRepository = repositoryRead;
    private readonly IConfiguration _configuration = configuration;

    private async Task Invoke()
    {
        await Task.Delay(10000);
        var orders = _orderReadRepository.GetAllByStatus(Domain.Enums.EOrderStatus.Reserved).ToArray();

        foreach (var order in orders)
        {
            var orderDate = order.CreatedAt;
            var currentDate = DateTime.UtcNow;
            var minutes = ( currentDate - orderDate ).TotalMinutes;
            if (minutes > _configuration.GetValue<int>("Order:CancelMinutes"))
            {
                _ = await _orderWriteRepository.UpdateAsync(order with
                {
                    Status = Domain.Enums.EOrderStatus.Closed,
                    UpdatedAt = DateTime.UtcNow
                }, cancellationToken: CancellationToken.None);

                _logger.LogInformation("Order {orderId} was closed", order.Id);
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

                await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError("Exception while class:{className} message: {errorMessage}", nameof(OrderStatusCancelWorker), ex.Message);
            }
        }
    }
}