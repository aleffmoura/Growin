namespace Growin.ApplicationService.Features.Orders.QueryHandlers;

using FunctionalConcepts.Errors;
using FunctionalConcepts.Results;
using Growin.ApplicationService.Features.Orders.Queries;
using Growin.Domain.Features;
using Growin.Domain.Interfaces.WriteRepositories;
using MediatR;
using System.Linq;
using System.Threading.Tasks;

public class OrderCollectionHandler(IOrderReadRepository repository) : IRequestHandler<OrderCollectionQuery, Result<IQueryable<Order>>>
{
    private readonly IOrderReadRepository _repository = repository;

    public Task<Result<IQueryable<Order>>> Handle(OrderCollectionQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var returned = Result.Of(_repository.GetAll());

            return Task.FromResult(returned);
        }
        catch (Exception ex)
        {
            UnhandledError error = ("Handled exception on get all orders", ex);
            return Task.FromResult(Result.Of<IQueryable<Order>>(error));
        }
    }
}
