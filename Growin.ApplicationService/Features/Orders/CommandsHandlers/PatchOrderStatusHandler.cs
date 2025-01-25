namespace Growin.ApplicationService.Features.Orders.CommandsHandlers;
using FunctionalConcepts;
using FunctionalConcepts.Errors;
using FunctionalConcepts.Results;
using Growin.ApplicationService.Features.Orders.Commands;
using Growin.Domain.Enums;
using Growin.Domain.Interfaces.WriteRepositories;

using MediatR;
using System.Threading;
using System.Threading.Tasks;

public class PatchOrderStatusHandler(
    IOrderWriteRepository orderWriteRepository,
    IOrderReadRepository orderReadRepository)
    : IRequestHandler<PatchOrderStatusCommand, Result<Success>>
{
    private readonly IOrderWriteRepository _orderWriteRepository = orderWriteRepository;
    private readonly IOrderReadRepository _orderReadRepository = orderReadRepository;

    public async Task<Result<Success>> Handle(PatchOrderStatusCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var orderOpt = await _orderReadRepository.GetAsync(request.OrderId, cancellationToken);

            return await orderOpt.MatchAsync(
                some: async (order) =>
                {
                    if(order is { Status: EOrderStatus.Reserved })
                    {
                        await _orderWriteRepository.UpdateAsync(order with
                        {
                            Status = EOrderStatus.Finished,
                            UpdatedAt = DateTime.UtcNow
                        }, cancellationToken);
                        return Result.Success;
                    }

                    return Result.Of<Success>(InvalidObjectError.New($"Order with status: {order.Status}"));
                },
                none: () => Result.Of<Success>(NotFoundError.New("Order not found"))
            );
        }
        catch (Exception ex)
        {
            UnhandledError error = ("Handled exception on create order", ex);
            return error;
        }
    }
}
