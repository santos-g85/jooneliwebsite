using webjooneli.Models.Entities;
public class MaintenanceMiddleware
{
    private readonly RequestDelegate _next;

    public MaintenanceMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var path = context.Request.Path.Value?.ToLower();

        // Allow toggle controller action during maintenance
        if (SiteSettings.IsMaintenanceMode && !(path?.StartsWith("/error/maintenance") ?? false))
        {
            context.Response.StatusCode = StatusCodes.Status503ServiceUnavailable;
            context.Response.ContentType = "text/html";

            await context.Response.WriteAsync(@"

                <!DOCTYPE html>
                <html lang=""en"">
                <head>
    <meta charset=""utf-8"" />
    <title>Service Unavailable</title>
    <link rel=""icon"" type=""image/x-icon"" href=""~/images/joonlogo--JfLTwqW.png"" />
    <meta name=""viewport"" content=""width=device-width, initial-scale=1"" />
    <link href=""https://cdn.jsdelivr.net/npm/bootstrap@5.3.2/dist/css/bootstrap.min.css"" rel=""stylesheet"" />
</head>
<body class=""d-flex align-items-center justify-content-center bg-light"" style=""height: 100vh;"">
    <div class=""text-center"">
        <h1 class=""display-4 text-danger fw-bold"">503</h1>
        <h3 class=""mb-3"">Service is temporarily unavailable due to maintenance.</h3>
        <p class=""mb-3"">Please try again later. We apologize for the inconvenience.</p>
    </div>
</body>
</html>


            ");
            return;
        }

        await _next(context);
    }
}
