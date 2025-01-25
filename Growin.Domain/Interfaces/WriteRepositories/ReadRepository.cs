namespace Growin.Domain.Interfaces.WriteRepositories;

using FunctionalConcepts.Options;
using System.Threading.Tasks;

public interface ReadRepository<TEntity> where TEntity : Entity<TEntity, Identifier>
{
    Task<Option<TEntity>> GetAsync(Identifier id, CancellationToken cancellationToken);
}
