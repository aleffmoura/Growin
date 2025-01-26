namespace Growin.Infra.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

internal class GrowinDbDesignTimeDbContextFactory : IDesignTimeDbContextFactory<GrowinDbContext>
{
    public GrowinDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<GrowinDbContext>();
        optionsBuilder.UseNpgsql(
            $"Host=192.168.1.209;Port=5432;Username=admin;Password=Sup3rS3cr3t;Database=growindb");

        return new(optionsBuilder.Options);
    }
}
