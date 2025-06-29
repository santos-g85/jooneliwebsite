using webjooneli.Models.Entities;
using Microsoft.AspNetCore.Mvc;

namespace webjooneli.Controllers
{
    public class SolutionsController : Controller
    {
        public IActionResult Index()
        {

            return View();
        }
        public IActionResult Details(string id)
        {
            var business = GetBusinessById(id);
            if (business == null)
                return NotFound();

            return View(business);
        }

        private BusinessModel? GetBusinessById(string id)
        {
            var businesses = new List<BusinessModel>
           {
               new BusinessModel
               {
                   Id = "corebanking",
                   Title = "360 Core Banking",
                   Description = "A fully integrated banking solution with modules for customers, loans, compliance, and reporting.",
                   Icon = "bi-bank2",
                   Svg = "bi bi-currency-exchange"
               },
               new BusinessModel
               {
                   Id = "trading",
                   Title = "Jooneli Trading",
                   Description = "High-performance online trading system with real-time analytics and multi-device access.",
                   Icon = "bi-graph-up-arrow",
                   Svg = "bi bi-bar-chart-line"
               },
               new BusinessModel
               {
                   Id = "multipledesk",
                   Title = "MultipleDesk",
                   Description = "Remote desktop management tool for IT and support teams to monitor and troubleshoot devices remotely.",
                   Icon = "bi-window-desktop",
                   Svg = "bi bi-pc-display"
               }
           };

            return businesses.FirstOrDefault(x => x.Id == id);
        }



    }
}
