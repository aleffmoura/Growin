using Growin.Api.ServicesExtension;
using Microsoft.AspNetCore.OData;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAntiforgery();
builder.Services.AddCors(opt =>
{
    opt.AddDefaultPolicy(op =>
    {
        op.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
    });
});

builder.Services
       .AddControllers(opt =>
       {
           opt.AddSwaggerMediaTypes();
       })
       .AddOData(opt => opt.Filter().Expand().Select().OrderBy().SetMaxTop(30).Count())
       .AddNewtonsoftJson(op =>
       {
           op.SerializerSettings.Formatting = Formatting.Indented;
           op.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
           op.SerializerSettings.Converters.Add(new StringEnumConverter());
           op.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
       });

builder.Services
       .AddEndpointsApiExplorer()
       .ConfigureSwagger();

builder.Host
       .ConfigureAutofac(builder.Configuration);

var app = builder.Build();
app.UseCors();
app.UseAntiforgery();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.DocumentTitle = "API DOC";
    });
}
app.UseODataQueryRequest();
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
