using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pharmacy.Data;
using Pharmacy.Models;
using Pharmacy.ViewsModels;
using System.Diagnostics;
using System.Security.Claims;
using X.PagedList;

namespace Pharmacy.Controllers
{
    
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ProductModels _ProductModels;
        private readonly DiscountModels _discountModels;
        private readonly QlpharmacyContext _qlpharmacyContext;
        private readonly CartModels _cartModels;
        private readonly CustomerModels _customerModels;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public HomeController(ILogger<HomeController> logger, ProductModels productModels, DiscountModels discountModels, QlpharmacyContext qlpharmacyContext, CartModels cart, CustomerModels customer, IHttpContextAccessor httpContextAccessor)
        {
            _logger = logger;
            _ProductModels = productModels; 
            _discountModels  = discountModels;
            _qlpharmacyContext = qlpharmacyContext;
            _cartModels = cart;
            _customerModels = customer;
            _httpContextAccessor = httpContextAccessor;
        }

        public const int Items_Per_Page = 12;
        public IActionResult Index(string search)
        {
           if(User.IsInRole("Admin"))
                return RedirectToAction("Index", "HomeAdmin", new { Areas = "Admin" });

            var listProducts = _ProductModels.GetProductsActive(search);
            listProducts = listProducts.Take(Items_Per_Page).ToList();
            ProductViewModels viewModel = new ProductViewModels
            {
              
                ListProduct = listProducts,
                DiscountPercentMap = _discountModels.GetDiscountPercentMap(listProducts, _qlpharmacyContext.Discounts.ToList())
            };

            var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            _httpContextAccessor.HttpContext.Session.SetInt32("counter", 0);
            ViewBag.countCart = null;
            if (userId != null)
            {
                var CustomerInfo = _customerModels.GetCustomer(userId);
                var Cart = _cartModels.getCartByCustomerId(CustomerInfo.CustomerId);
                int countCart = _cartModels.TotalQuantityCartDetail(Cart.CartId);
                _httpContextAccessor.HttpContext.Session.SetInt32("counter", countCart);
                ViewBag.countCart = _httpContextAccessor.HttpContext.Session.GetInt32("counter");
            }
          
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