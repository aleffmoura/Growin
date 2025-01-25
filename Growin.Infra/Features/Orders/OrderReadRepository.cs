namespace Growin.Infra.Features.Orders;

using Growin.Domain.Enums;
using Growin.Domain.Features;
using Growin.Domain.Interfaces.WriteRepositories;
using Growin.Infra.Context;
using Microsoft.EntityFrameworkCore;
using System.Linq;

public class OrderReadRepository(GrowinDbContext dbContext)
    : ReadRepository<Order>(dbContext), IOrderReadRepository
{
    public IQueryable<Order> GetAllByStatus(EOrderStatus status)
        => _dbContext.Orders.AsNoTracking().Where(x => x.Status == status);
}