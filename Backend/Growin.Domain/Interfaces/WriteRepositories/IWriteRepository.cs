namespace Growin.Domain.Interfaces.WriteRepositories;

using FunctionalConcepts;
using System.Threading.Tasks;

public interface IWriteRepository<TEntity> where TEntity : Entity<TEntity, Identifier>
{
    Task<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken);
    Task<Success> UpdateAsync(TEntity entity, CancellationToken cancellationToken);
}
