using Microsoft.AspNetCore.Mvc;

namespace Pharmacy.Controllers
{
    public class Contact : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
