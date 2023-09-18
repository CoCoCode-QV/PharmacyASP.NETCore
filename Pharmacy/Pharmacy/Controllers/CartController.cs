using Microsoft.AspNetCore.Mvc;

namespace Pharmacy.Controllers
{
    public class CartController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
