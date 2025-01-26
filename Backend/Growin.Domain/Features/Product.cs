namespace Growin.Domain.Features;

public record Product : Entity<Product, Identifier>
{
    public required string Name { get; init; }
    public required long QuantityInStock { get; init; }
    public virtual List<Order> Orders { get; set; } = [];
}
