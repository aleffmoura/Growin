namespace Growin.Domain.Bases;

public record class Entity<TEntity, TId>
    where TEntity : notnull, Entity<TEntity, TId>
    where TId : notnull
{
    public required TId Id { get; init; }
    public required DateTime CreatedAt { get; init; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; } = null;
}
