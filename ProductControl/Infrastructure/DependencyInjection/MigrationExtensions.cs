using Microsoft.EntityFrameworkCore;
using ProductControl.Infrastructure.Persistence;

namespace ProductControl.Infrastructure.DependencyInjection;

public static class MigrationExtensions
{
    public static WebApplication ApplyMigrations(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var logger = scope.ServiceProvider.GetRequiredService<ILoggerFactory>().CreateLogger("Migration");

        try
        {
            var db = scope.ServiceProvider.GetRequiredService<ProductDbContext>();
            db.Database.Migrate();
            logger.LogInformation("Migrations applied successfully.");
        }
        catch (Exception ex)
        {
            /*if DB is not connected*/
            logger.LogError(ex.Message, "Failed to apply migrations. Make sure the database container is running.");
        }

        return app;
    }
}
