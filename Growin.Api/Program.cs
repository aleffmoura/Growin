using Growin.Api.ServicesExtension;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAntiforgery();
builder.Services.AddCors(opt =>
{
    opt.AddDefaultPolicy(op =>
    {
        op.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
    });
});

builder.Host
       .ConfigureAutofac(builder.Configuration);

var app = builder.Build();
app.UseCors();
app.UseAntiforgery();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
