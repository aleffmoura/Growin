namespace Growin.Domain.Interfaces.WriteRepositories;

using Growin.Domain.Features;

public interface IOrderReadRepository : IReadRepository<Order>
{
    IQueryable<Order> GetAllByStatus(EOrderStatus status);
}