using Microsoft.AspNetCore.Mvc;

namespace Pharmacy.Controllers
{
    public class LoginController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
