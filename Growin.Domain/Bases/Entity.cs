namespace Growin.Domain.Bases;

public record class Entity<TEntity, TId>
    where TEntity : notnull, Entity<TEntity, TId>
    where TId : notnull
{
    public required TId Id { get; init; }
}
