using Microsoft.AspNetCore.Mvc;

namespace Pharmacy.Controllers
{
    public class Payment : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
