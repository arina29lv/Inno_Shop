using ProductControl.Infrastructure.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddDatabaseService(builder.Configuration)
    .AddApplicationServices()
    .AddScopedServices()
    .AddValidationService()
    .AddRabbitMq()
    .AddJwtAuthentication(builder.Configuration)
    .AddSwaggerDocumentation();

var app = builder.Build();

app.UseSwaggerDocumentation()
    .UseAuth()
    .ApplyMigrations() /* works only with connected DB - simplify migrations for docker's DB container */
    .MapControllers();

app.Run();
