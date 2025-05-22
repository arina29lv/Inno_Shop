using Microsoft.EntityFrameworkCore;
using UserControl.Infrastructure.Persistence;

namespace UserControl.Infrastructure.DependencyInjection;

public static class MigrationExtensions
{
    public static WebApplication ApplyMigrations(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<UserDbContext>();
        db.Database.Migrate();
        return app;
    }
}