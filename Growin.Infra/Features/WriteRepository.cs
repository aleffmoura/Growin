namespace Growin.Infra.Features;

using FunctionalConcepts;
using FunctionalConcepts.Results;
using Growin.Domain.Bases;
using Growin.Domain.Interfaces.WriteRepositories;
using Growin.Infra.Context;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

public class WriteRepository<TEntity>(GrowinDbContext dbContext) : IWriteRepository<TEntity>
    where TEntity : Entity<TEntity, Identifier>
{
    private readonly GrowinDbContext _dbContext = dbContext;

    public async Task<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken)
    {
        var entityEntry = await _dbContext.Set<TEntity>().AddAsync(entity, cancellationToken);
        await _dbContext.SaveChangesAsync();
        return entityEntry.Entity;
    }

    public async Task<Success> UpdateAsync(TEntity entity, CancellationToken cancellationToken)
    {
        var tracked = _dbContext.Update(entity);
        tracked.State = EntityState.Modified;

        await _dbContext.SaveChangesAsync(cancellationToken);
        tracked.State = EntityState.Detached;
        return Result.Success;
    }
}
