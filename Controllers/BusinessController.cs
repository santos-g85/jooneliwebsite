using jooneliwebsite.Models;
using Microsoft.AspNetCore.Mvc;

namespace jooneliwebsite.Controllers
{
    public class BusinessController : Controller
    {
        private readonly AppSettings _appSettings;

        public BusinessController(AppSettings appSettings)
        {
            _appSettings = appSettings;
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Multipledesk()
        {

            string? Multipledeskurl = _appSettings.Multipledeskurl;

            if (string.IsNullOrWhiteSpace(Multipledeskurl))
            {
                Multipledeskurl = "jooneli.com";
            }
            return Redirect(Multipledeskurl);
        }
    }
}
