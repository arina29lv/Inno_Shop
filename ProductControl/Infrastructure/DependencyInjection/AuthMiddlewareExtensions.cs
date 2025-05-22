namespace ProductControl.Infrastructure.DependencyInjection;

public static class AuthMiddlewareExtensions
{
    public static WebApplication UseAuth(this WebApplication app)
    {
        app.UseAuthentication();
        app.UseAuthorization();
        return app;
    }
}