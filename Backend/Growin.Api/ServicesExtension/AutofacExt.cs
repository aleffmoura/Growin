namespace Growin.Api.ServicesExtension;

using Autofac.Extensions.DependencyInjection;
using Autofac;
using Growin.Api.Modules;
using MediatR.Extensions.Autofac.DependencyInjection.Builder;
using MediatR.Extensions.Autofac.DependencyInjection;

public static class AutofacExt
{
    public static IHostBuilder ConfigureAutofac(this IHostBuilder hostBuilder, IConfigurationRoot cfgRoot)
    {
        return hostBuilder
            .UseServiceProviderFactory(new AutofacServiceProviderFactory())
            .ConfigureContainer<ContainerBuilder>((context, containerBuilder) =>
            {
                var configuration = MediatRConfigurationBuilder
                                    .Create(typeof(Program).Assembly)
                                    .WithAllOpenGenericHandlerTypesRegistered()
                                    .WithRegistrationScope(RegistrationScope.Scoped)
                                    .Build();

                containerBuilder.RegisterModule(new FluentValidationModule());
                containerBuilder.RegisterModule(new GlobalModule<Program>(cfgRoot));
                containerBuilder.RegisterModule(new MediatRModule());

                containerBuilder.RegisterMediatR(configuration);
                containerBuilder.Register(r => containerBuilder).AsSelf().InstancePerLifetimeScope();
            })
            .ConfigureHostOptions(o => o.ShutdownTimeout = TimeSpan.FromSeconds(60));
    }
}
