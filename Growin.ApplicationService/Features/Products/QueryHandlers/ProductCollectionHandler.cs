namespace Growin.ApplicationService.Features.Products.QueryHandlers;
using FunctionalConcepts.Errors;
using FunctionalConcepts.Results;
using Growin.ApplicationService.Features.Products.Queries;
using Growin.Domain.Features;
using Growin.Domain.Interfaces.WriteRepositories;

using MediatR;
using System;
using System.Linq;
using System.Threading.Tasks;

public class ProductCollectionHandler(IProductReadRepository repository) : IRequestHandler<ProductCollectionQuery, Result<IQueryable<Product>>>
{
    private readonly IProductReadRepository _repository = repository;

    public Task<Result<IQueryable<Product>>> Handle(ProductCollectionQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var returned = Result.Of(_repository.GetAll());

            return Task.FromResult(returned);
        }
        catch (Exception ex)
        {
            UnhandledError error = ("Handled exception on get all products", ex);
            return Task.FromResult(Result.Of<IQueryable<Product>>(error));
        }
    }
}
