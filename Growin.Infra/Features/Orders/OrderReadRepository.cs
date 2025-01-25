namespace Growin.Infra.Features.Orders;

using Growin.Domain.Features;
using Growin.Domain.Interfaces.WriteRepositories;
using Growin.Infra.Context;

public class OrderReadRepository(GrowinDbContext dbContext)
    : ReadRepository<Order>(dbContext), IOrderReadRepository;