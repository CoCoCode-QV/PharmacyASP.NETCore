using Microsoft.AspNetCore.Mvc;

namespace Pharmacy.Controllers
{
    public class AboutController : Controller
    {
        public IActionResult Index()
        {
            ViewBag.countCart = HttpContext.Session.GetInt32("counter");
            return View();
        }
    }
}
