using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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

            if (User.IsInRole("Staff"))
                return RedirectToAction("Index", "Product", new { Areas = "Admin" });


            var listProducts = _ProductModels.GetProductsActive(search);
            var listProductsCost = listProducts
                        .SelectMany(product => _qlpharmacyContext.ProductCosts
                        .Where(pc => pc.ProductId == product.ProductId && pc.CostActive && pc.ProductInventory > 0 && product.ProductActive == true && pc.ProductExpiryDate > DateTime.Now).Include(pc =>pc.ProductDiscounts).Include(pc =>pc.Product))
                        .ToList();
            // Tính toán phần trăm chiết khấu cho mỗi sản phẩm
            var discountPercentMap = _discountModels.GetDiscountPercentMap(listProductsCost, _qlpharmacyContext.Discounts.ToList());

            // Sắp xếp danh sách sản phẩm dựa trên phần trăm chiết khấu từ cao đến thấp
            listProductsCost = listProductsCost
                .OrderByDescending(pc => discountPercentMap.ContainsKey(pc.CostId) ? discountPercentMap[pc.CostId] : 0)
                .ThenByDescending(pc => pc.Product.ProductName) // Sắp xếp phụ theo tên sản phẩm nếu cần
                .Take(Items_Per_Page)
                .ToList();
            listProductsCost = listProductsCost.Take(Items_Per_Page).ToList();
            ProductViewModels viewModel = new ProductViewModels
            {
                ListProductCost = listProductsCost,
                DiscountPercentMap = discountPercentMap
            };

            //listProductsCost = listProductsCost.Take(Items_Per_Page).ToList();
            //ProductViewModels viewModel = null;
            // viewModel = new ProductViewModels
            //{
            //    ListProductCost = listProductsCost,
            //    DiscountPercentMap = _discountModels.GetDiscountPercentMap(listProductsCost, _qlpharmacyContext.Discounts.ToList())
            //};

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