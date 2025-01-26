namespace Growin.ApplicationService.Features.Orders.Queries;
using FunctionalConcepts.Results;
using Growin.Domain.Features;
using MediatR;
using System.Linq;

public class OrderCollectionQuery : IRequest<Result<IQueryable<Order>>>
{
}
