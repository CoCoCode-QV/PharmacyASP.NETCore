using Microsoft.AspNetCore.Mvc;
using Pharmacy.Data;

namespace Pharmacy.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CategoryController : Controller
    {

        private readonly QlpharmacyContext _context;

        public CategoryController(QlpharmacyContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
          
            var categories = _context.Categories.ToList();
            return View(categories);
        }
    }
}
