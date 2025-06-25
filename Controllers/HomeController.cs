using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using webjooneli.Models.ViewModels;
using webjooneli.Services.Implementation;

namespace webjooneli.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UserSessionService _sessionservice;
        public HomeController(ILogger<HomeController> logger, UserSessionService sessionService)
        {
            _logger = logger;
           _sessionservice = sessionService;
        }

        public async Task<IActionResult> Index()
        {
            await _sessionservice.EnsureSession();
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
