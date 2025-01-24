namespace Growin.ApplicationService.Features.Orders.Commands;

using FunctionalConcepts.Results;
using MediatR;

public record CreateOrderCommand : IRequest<Result<long>>
{
    public required long ProductId { get; init; }
    public required ulong Quantity { get; init; }
}
