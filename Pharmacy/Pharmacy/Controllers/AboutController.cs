using Microsoft.AspNetCore.Mvc;

namespace Pharmacy.Controllers
{
    public class AboutController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
