using Microsoft.EntityFrameworkCore;
using ProductControl.Infrastructure.Persistence;

namespace ProductControl.Infrastructure.DependencyInjection;

public static class MigrationExtensions
{
    public static WebApplication ApplyMigrations(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ProductDbContext>();
        db.Database.Migrate();
        return app;
    }
}
