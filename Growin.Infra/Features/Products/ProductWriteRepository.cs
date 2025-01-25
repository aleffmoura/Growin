namespace Growin.Infra.Features.Products;
using Growin.Domain.Features;
using Growin.Domain.Interfaces.WriteRepositories;

using Growin.Infra.Context;

public class ProductWriteRepository(GrowinDbContext dbContext)
    : WriteRepository<Product>(dbContext), IProductWriteRepository
{
}
