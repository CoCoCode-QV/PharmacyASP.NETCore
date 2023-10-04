using Microsoft.AspNetCore.Mvc;

namespace Pharmacy.Controllers
{
    public class CustomerInfoController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
