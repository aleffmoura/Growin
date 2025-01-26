namespace Growin.Api.ServicesExtension;

using Growin.Api.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Formatter;
using Microsoft.Net.Http.Headers;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Models;

public static class SwaggerExt
{
    public static MvcOptions AddSwaggerMediaTypes(this MvcOptions options)
    {
        foreach (var outputFormatter in options.OutputFormatters.OfType<ODataOutputFormatter>().Where(_ => _.SupportedMediaTypes.Count == 0))
        {
            outputFormatter.SupportedMediaTypes.Add(new MediaTypeHeaderValue("application/prs.odatatestxx-odata"));
        }
        foreach (var inputFormatter in options.InputFormatters.OfType<ODataInputFormatter>().Where(_ => _.SupportedMediaTypes.Count == 0))
        {
            inputFormatter.SupportedMediaTypes.Add(new MediaTypeHeaderValue("application/prs.odatatestxx-odata"));
        }

        return options;
    }

    public static IServiceCollection ConfigureSwagger(this IServiceCollection services)
        => services.AddSwaggerGen(opts =>
        {
            opts.OperationFilter<RemoveAntiforgeryHeaderOperationFilter>();
            opts.OperationFilter<CustomHeaderSwaggerAttribute>();
            opts.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Growin API",
                Version = "v1",
                Description = "API voltada para listagem e informações de pedidos e produtos",
                Contact = new OpenApiContact
                {
                    Name = "Aleff Moura",
                    Email = "aleffmds@gmail.com",
                    Url = new Uri("https://www.linkedin.com/in/aleff-moura/"),
                    Extensions =
                    {
                        { "x-company", new OpenApiString("Aleff Solutions") }
                    }
                },
                License = new OpenApiLicense
                {
                    Name = "License",
                    Url = new Uri("https://google.com.br")
                },
                TermsOfService = new Uri("https://mail.google.com"),
                Extensions = new Dictionary<string, IOpenApiExtension>
                {
                    { "x-company", new OpenApiString("Aleff Moura") },
                    { "x-contact", new OpenApiString("aleffmds@gmail.com") }
                }
            });

            opts.CustomSchemaIds(x => x.FullName);
        })
       .Configure<RequestLocalizationOptions>(op => op.DefaultRequestCulture = new Microsoft.AspNetCore.Localization.RequestCulture("pt-BR"));
}
