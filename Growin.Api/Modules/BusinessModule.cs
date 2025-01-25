namespace Growin.Api.Modules;

using Autofac;
using AutoMapper;
using Growin.ApplicationService;
using Growin.Domain.Interfaces.WriteRepositories;
using Growin.Infra.Context;
using Growin.Infra.Features.Orders;
using Growin.Infra.Features.Products;
using Microsoft.EntityFrameworkCore;

public class GlobalModule<TProgram> : Module
{
    IConfigurationRoot Configuration { get; }

    public GlobalModule(IConfigurationRoot configuration)
    {
        Configuration = configuration;
    }

    protected override void Load(ContainerBuilder builder)
    {
        builder.Register(_ => Configuration)
               .As<IConfigurationRoot>()
               .InstancePerLifetimeScope();

        builder.RegisterType<OrderWriteRepository>()
               .As<IOrderWriteRepository>()
               .InstancePerLifetimeScope();

        builder.RegisterType<OrderReadRepository>()
               .As<IOrderReadRepository>()
               .InstancePerLifetimeScope();

        builder.RegisterType<ProductWriteRepository>()
               .As<IProductWriteRepository>()
               .InstancePerLifetimeScope();

        builder.RegisterType<ProductReadRepository>()
               .As<IProductReadRepository>()
               .InstancePerLifetimeScope();

        builder.Register(ctx =>
        {
            var strConn = Configuration.GetConnectionString("PrincipalDb");
            var opt = new DbContextOptionsBuilder<GrowinDbContext>()
                                             .UseNpgsql(strConn)
                                             .Options;
            return new GrowinDbContext(opt);
        }).AsSelf()
        .InstancePerLifetimeScope();

        builder.Register(ctx =>
        {
            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.AddMaps(typeof(TProgram));
                cfg.AddMaps(typeof(ApplicationAssembly));
            });

            return configuration.CreateMapper();
        }).As<IMapper>()
          .InstancePerLifetimeScope();
    }
}