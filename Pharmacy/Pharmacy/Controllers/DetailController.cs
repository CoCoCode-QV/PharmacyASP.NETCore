using Microsoft.AspNetCore.Mvc;

namespace Pharmacy.Controllers
{
    public class DetailController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
