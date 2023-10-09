using Microsoft.AspNetCore.Mvc;

namespace Pharmacy.Controllers
{
    public class Contact : Controller
    {
        public IActionResult Index()
        {
            ViewBag.countCart = HttpContext.Session.GetInt32("counter");
            return View();
        }
    }
}
