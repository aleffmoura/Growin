namespace Growin.Infra.Features.Products;
using Growin.Domain.Features;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

internal class ProductEntityConfiguration : IEntityTypeConfiguration<Product>
{
    const string TABLE_NAME = "Products";
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable(TABLE_NAME);
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Name)
               .HasMaxLength(250)
               .IsRequired();

        builder.Property(e => e.QuantityInStock)
               .IsRequired();

        builder.Property(e => e.CreatedAt)
               .HasColumnType("timestamp with time zone")
               .IsRequired();

        builder.Property(e => e.UpdatedAt)
               .HasColumnType("timestamp with time zone");

        builder.HasMany(e => e.Orders)
               .WithOne(p => p.Product)
               .HasForeignKey(x => x.ProductId);

        builder.HasData(new Product
        {
            Id = 1,
            Name = "Product Test",
            QuantityInStock = 100,
            CreatedAt = DateTime.UtcNow,
        });
    }
}
