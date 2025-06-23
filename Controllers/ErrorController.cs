using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;

namespace WebJooneli.Controllers
{
    public class ErrorController : Controller
    {
        private readonly ILogger<ErrorController> _logger;

        
        public ErrorController(ILogger<ErrorController> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        
        [Route("Error/404")]
        public IActionResult Error404()
        {
            _logger.LogError("Page not found - 404 error.");
            return View("Error404");  
        }

        // 503 Error (Service Unavailable)
        [Route("Error/503")]
        public IActionResult Error503()
        {
            _logger.LogError("Service is temporarily unavailable - 503 error.");
            return View("Maintenance");  
        }

        // Generic error handler that can handle more than just 404 and 503
        [Route("Error/{statusCode}")]
        public IActionResult GenericHandler(int statusCode)
        {
            // Log error with the status code
            _logger.LogError($"Encountered error with status code: {statusCode}");

            // Handle common HTTP status codes
            switch (statusCode)
            {
                case 400:
                    return View("BadRequest"); // A view for bad request errors (400)
                case 404:
                    return RedirectToAction("Error404");
                case 503:
                    return RedirectToAction("Error503");
                case 500:
                    return View("Maintenance"); // A custom view for 500 Internal Server Errors
                default:
                    return View("GenericError"); // Default for any unhandled error
            }
        }

        // for unhandled exceptions if needed
        [Route("Error/Exception")]
        public IActionResult HandleException()
        {
            _logger.LogError("Unhandled exception occurred.");
            return View("GenericError"); 
        }
    }
}
