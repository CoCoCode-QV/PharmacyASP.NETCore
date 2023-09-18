using Microsoft.AspNetCore.Mvc;

namespace Pharmacy.Controllers
{
    public class ProductsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
