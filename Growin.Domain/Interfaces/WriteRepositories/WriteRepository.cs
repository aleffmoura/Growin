namespace Growin.Domain.Interfaces.WriteRepositories;

using System.Threading.Tasks;

public interface WriteRepository<TEntity> where TEntity : Entity<TEntity, Identifier>
{
    Task<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken);
}
