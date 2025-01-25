namespace Growin.Infra.Features;

using FunctionalConcepts.Options;
using Growin.Domain.Bases;
using Growin.Domain.Interfaces.WriteRepositories;
using Growin.Infra.Context;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

public class ReadRepository<TEntity>(GrowinDbContext dbContext) : IReadRepository<TEntity>
    where TEntity : Entity<TEntity, Identifier>
{
    protected readonly GrowinDbContext _dbContext = dbContext;

    public IQueryable<TEntity> GetAll()
        => _dbContext.Set<TEntity>().AsNoTracking().AsQueryable();

    public async Task<Option<TEntity>> GetAsync(long id, CancellationToken cancellationToken)
    {
        var entity =
            await _dbContext.Set<TEntity>()
                            .AsNoTracking()
                            .Where(x => x.Id == id)
                            .FirstOrDefaultAsync();

        return entity ?? new Option<TEntity>();
    }
}
