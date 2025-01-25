namespace Growin.Domain.Features;

public record Order : Entity<Order, Identifier>
{
    public required Identifier ProductId { get; init; }
    public required long Quantity { get; init; }
    public required EOrderStatus Status { get; init; }
}
