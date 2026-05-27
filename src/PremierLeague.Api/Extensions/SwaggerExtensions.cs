using Microsoft.OpenApi.Models;

namespace PremierLeague.Api.Extensions;

public static class SwaggerExtensions
{
    public static IServiceCollection AddSwaggerDocumentation(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Premier League Stats API",
                Version = "v1",
                Description = "A professional REST API for Premier League statistics, standings, and match results.",
                Contact = new OpenApiContact { Name = "API Support", Email = "support@premierleagueapi.dev" },
                License = new OpenApiLicense { Name = "MIT", Url = new Uri("https://opensource.org/licenses/MIT") }
            });

            options.DescribeAllParametersInCamelCase();
        });

        return services;
    }

    public static WebApplication UseSwaggerDocumentation(this WebApplication app)
    {
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "Premier League Stats API v1");
            c.RoutePrefix = "swagger";
            c.DocumentTitle = "PL Stats API";
        });

        return app;
    }
}
