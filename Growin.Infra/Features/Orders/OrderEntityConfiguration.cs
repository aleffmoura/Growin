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
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Quantity).IsRequired();
        builder.Property(p => p.Status).IsRequired();
        builder.Property(p => p.ProductId)
               .IsRequired();

        builder.HasIndex(p => p.ProductId);
    }
}
