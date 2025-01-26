namespace Growin.Infra.Features.Orders;

using Growin.Domain.Features;
using Growin.Domain.Interfaces.WriteRepositories;
using Growin.Infra.Context;

public class OrderWriteRepository(GrowinDbContext dbContext)
    : WriteRepository<Order>(dbContext), IOrderWriteRepository
{
}
