using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pharmacy.Data;
using Pharmacy.Models;
using Pharmacy.ViewsModels;
using System.Diagnostics;
using X.PagedList;

namespace Pharmacy.Controllers
{
    
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ProductModels _ProductModels;
        private readonly DiscountModels _discountModels;
        private readonly QlpharmacyContext _qlpharmacyContext;
        public HomeController(ILogger<HomeController> logger, ProductModels productModels, DiscountModels discountModels, QlpharmacyContext qlpharmacyContext)
        {
            _logger = logger;
            _ProductModels = productModels; 
            _discountModels  = discountModels;
            _qlpharmacyContext = qlpharmacyContext;
        }

        public const int Items_Per_Page = 12;
        public IActionResult Index(string search)
        {
           if(User.IsInRole("Admin"))
                return RedirectToAction("Index", "HomeAdmin", new { Areas = "Admin" });

            var listProducts = _ProductModels.GetProductsActive(search);

            ProductViewModels viewModel = new ProductViewModels
            {
              
                ListProduct = listProducts,
                DiscountPercentMap = _discountModels.GetDiscountPercentMap(listProducts, _qlpharmacyContext.Discounts.ToList())
            };
            return View(viewModel);
        }

        public IActionResult Privacy()
        {   
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}