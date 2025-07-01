using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using webjooneli.Models.ViewModels;
using webjooneli.Repository.Implementations;
using webjooneli.Repository.Interfaces;
using webjooneli.Services.Implementation;

namespace webjooneli.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly INewsRepository _newsRepository;
        public HomeController(ILogger<HomeController> logger,
            INewsRepository newsRepository)
        {
            _logger = logger;
            _newsRepository = newsRepository;
        }

        public async Task<IActionResult> Index()
        {
            var news = await _newsRepository.GetAllNewsAsync();
            return View(news);
        }

        
        [ResponseCache(Duration = 432000, 
            Location = ResponseCacheLocation.Any, 
            NoStore = false)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

    }
}
