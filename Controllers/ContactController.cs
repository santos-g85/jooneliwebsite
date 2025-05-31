using Microsoft.AspNetCore.Mvc;

namespace jooneliwebsite.Controllers
{
    public class ContactController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
