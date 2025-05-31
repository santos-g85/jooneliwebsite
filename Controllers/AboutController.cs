using Microsoft.AspNetCore.Mvc;

namespace jooneliwebsite.Controllers
{
    public class AboutController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
