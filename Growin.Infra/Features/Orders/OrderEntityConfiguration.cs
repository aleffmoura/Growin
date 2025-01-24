namespace Growin.Infra.Features.Orders;

using Growin.Domain.Features;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

internal class OrderEntityConfiguration : IEntityTypeConfiguration<Order>
{
    const string TABLE_NAME = "Orders";
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.ToTable(TABLE_NAME);
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Quantity).IsRequired();
        builder.Property(e => e.Status).IsRequired();
        builder.Property(e => e.ProductId).IsRequired();
    }
}
