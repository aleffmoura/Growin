namespace Growin.ApplicationService.Features.Products.Queries;
using FunctionalConcepts.Results;
using Growin.Domain.Features;

using MediatR;
using System.Linq;

public class ProductCollectionQuery : IRequest<Result<IQueryable<Product>>>
{
}
