namespace Growin.Domain.Features;

public record Order : Entity<Identifier>
{
    public required Identifier ProductId { get; init; }
    public required uint Quantity { get; init; }
    public required EOrderStatus Status { get; init; }
}
