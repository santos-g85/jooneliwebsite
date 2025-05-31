using Microsoft.AspNetCore.Mvc;

namespace jooneliwebsite.Controllers
{
    public class CareerController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
