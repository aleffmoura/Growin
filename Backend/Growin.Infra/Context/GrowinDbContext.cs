﻿namespace Growin.Infra.Context;

using Growin.Domain.Features;
using Growin.Infra.Features.Orders;
using Growin.Infra.Features.Products;
using Microsoft.EntityFrameworkCore;

public class GrowinDbContext : DbContext
{
    public virtual DbSet<Order> Orders { get; set; }
    public virtual DbSet<Product> Products { get; set; }

    public GrowinDbContext(DbContextOptions<GrowinDbContext> options) : base(options)
    {
        try
        {
            Database.Migrate();
        }
        catch{ }

        ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
        => base.OnModelCreating(
                modelBuilder.ApplyConfiguration(new OrderEntityConfiguration())
                            .ApplyConfiguration(new ProductEntityConfiguration()));

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => base.OnConfiguring(optionsBuilder.UseLazyLoadingProxies());
}
