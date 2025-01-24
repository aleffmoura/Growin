namespace Growin.Domain.Features;

public record Product : Entity<Product, Identifier>
{
    public required string Name { get; set; }
    
    public virtual List<Order> Orders { get; set; } = [];
}
