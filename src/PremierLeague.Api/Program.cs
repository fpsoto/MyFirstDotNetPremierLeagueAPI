using PremierLeague.Api.Extensions;
using PremierLeague.Api.Middleware;
using PremierLeague.Application;
using PremierLeague.Infrastructure;
using PremierLeague.Infrastructure.Seeders;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Serilog configured from appsettings.json
builder.Host.UseSerilog((ctx, lc) => lc.ReadFrom.Configuration(ctx.Configuration));

builder.Services.AddControllers();
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddSwaggerDocumentation();
builder.Services.AddHealthChecks();

var app = builder.Build();

app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseSerilogRequestLogging(opts => opts.MessageTemplate =
    "{RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000} ms");

if (app.Environment.IsDevelopment())
    app.UseSwaggerDocumentation();

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.MapHealthChecks("/health");

// Seed database on startup
using (var scope = app.Services.CreateScope())
{
    var seeder = scope.ServiceProvider.GetRequiredService<DatabaseSeeder>();
    await seeder.SeedAsync();
}

app.Run();

// Required for WebApplicationFactory in integration tests
public partial class Program { }
