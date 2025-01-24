namespace Growin.Domain.Bases;

public record class Entity<TId> where TId : notnull
{
    public required TId Id { get; init; }
}
