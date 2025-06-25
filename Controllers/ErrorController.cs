using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

[Route("Error")]
public class ErrorController : Controller
{
    private readonly ILogger<ErrorController> _logger;
    public ErrorController(ILogger<ErrorController> logger)
    {
        _logger = logger;
    }

   

    [Route("404")]
    public IActionResult Error404()
    {
        var originalPath = HttpContext.Features.Get<IStatusCodeReExecuteFeature>()?.OriginalPath;
        _logger.LogWarning("404 Not Found: {Path}", originalPath);
        return View("Error404");
    }

    [Route("503")]
    public IActionResult Error503()
    {
        _logger.LogError("503 Service Unavailable");
        return View("Maintenance");
    }

    [Route("{statusCode:int}")]
    public IActionResult StatusCodeHandler(int statusCode)
    {
        
        _logger.LogError("HTTP {StatusCode} error occurred", statusCode);

        return statusCode switch
        {
            404 => RedirectToAction("Error404"),
            503 => RedirectToAction("Error503"),
            >= 500 => View("Maintenance"),
            _ => View("GenericError")
        };
    }

    [Route("HandleException")]
    public IActionResult HandleException()
    {
        var exceptionHandlerPathFeature = HttpContext.Features.Get<IExceptionHandlerPathFeature>();
        _logger.LogError(exceptionHandlerPathFeature?.Error, "Unhandled exception occurred");
        return View("GenericError");
    }

    [Route("maintenance/toggle/{status}")]
    public IActionResult Maintenance(string status)
    {
        if (status != "on" && status != "off")
        {
            return BadRequest("Invalid status. Use 'on' or 'off'.");
        }
        MaintenanceService.IsMaintenaceActive = status == "on";
        _logger.LogInformation($"switch toggled to: {status}");
        return RedirectToAction("Index", "Home"); 
    }
}

public static class MaintenanceService
{
    private static bool _isMaintenanceActive = false;

    public static bool IsMaintenaceActive
    {
        get => _isMaintenanceActive;
        set => _isMaintenanceActive = value;
    }

}