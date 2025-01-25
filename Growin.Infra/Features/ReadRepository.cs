namespace Growin.Infra.Features;

using FunctionalConcepts.Options;
using Growin.Domain.Bases;
using Growin.Domain.Interfaces.WriteRepositories;
using Growin.Infra.Context;
using System.Threading;
using System.Threading.Tasks;

public class ReadRepository<TEntity>(GrowinDbContext dbContext) : IReadRepository<TEntity>
    where TEntity : Entity<TEntity, Identifier>
{
    private readonly GrowinDbContext _dbContext = dbContext;

    public async Task<Option<TEntity>> GetAsync(long id, CancellationToken cancellationToken)
    {
        var entity =
            await _dbContext.Set<TEntity>()
                            .FindAsync([id], cancellationToken);

        return entity ?? new Option<TEntity>();
    }
}
