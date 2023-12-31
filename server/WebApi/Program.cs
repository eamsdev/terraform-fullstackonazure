using Application;
using Domain;
using Infrastructure;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using WebApi;

var builder = WebApplication.CreateBuilder(args);
var env = builder.Environment;

// Add services to the container.
builder.Services
    .AddWebServices()
    .AddInfrastructureServices(builder.Configuration)
    .AddApplicationServices()
    .AddDomainServices()
    .AddHealthChecks();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (env.IsDevelopment())
{
    app.UseOpenApi();
    app.UseSwaggerUi3();
    
    using var scope = app.Services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    if (dbContext.Database.IsRelational())
    {
        await dbContext.Database.MigrateAsync();
    }
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.UseExceptionHandler();
app.MapControllers().RequireAuthorization();
app.MapHealthChecks("/health");
app.Run();

namespace WebApi
{
    public partial class Program { }
}