namespace Growin.ApplicationService.Features.Orders.Commands;

using FunctionalConcepts;
using FunctionalConcepts.Results;
using MediatR;

public record PatchOrderStatusCommand : IRequest<Result<Success>>
{
    public required long OrderId { get; init; }
}
