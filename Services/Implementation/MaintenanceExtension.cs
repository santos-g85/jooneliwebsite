using Microsoft.AspNetCore.Builder;

public static class MaintenanceMiddlewareExtensions
{
    public static IApplicationBuilder UseMaintenanceMode(this IApplicationBuilder app)
    {
        return app.UseMiddleware<MaintenanceMiddleware>();
    }
}
