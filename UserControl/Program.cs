using UserControl.Infrastructure.DependencyInjection;

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
    .ApplyMigrations() 
    .MapControllers();

app.Run();