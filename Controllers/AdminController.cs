using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using webjooneli.Models.Entities;
using webjooneli.Models.ViewModels;
using webjooneli.Repository.Interfaces;

namespace webjooneli.Controllers
{
    [Route("hidden-dashboard")]
    public class AdminController : Controller
    {
        private readonly ISessionRepository _sessionRepository;
        private ILogger<AdminController> _logger;
        public AdminController(ILogger<AdminController> logger,
            ISessionRepository sessionRepository)
        {
            _logger = logger;
            _sessionRepository = sessionRepository;
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Index()
        {
            
            return View();
        }
    }
}
