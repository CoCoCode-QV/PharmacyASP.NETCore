using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pharmacy.Data;
using Pharmacy.Models;
using Pharmacy.ViewsModels;

namespace Pharmacy.Controllers
{
    public class DetailController : Controller
    {
        private readonly ProductModels _products;
        private readonly QlpharmacyContext _context;
        private readonly DiscountModels _discount;

        public DetailController(ProductModels products, QlpharmacyContext context, DiscountModels discount) { 
            _context = context;
            _products = products;
            _discount = discount;
        }

        public IActionResult Index(int id)
        {
            Product product = _products.GetProduct(id);
            Discount discount = _context.Discounts.FirstOrDefault(p => p.DiscountId == product.DiscountId);
            Supplier supplier = _context.Suppliers.FirstOrDefault(p => p.SupplierId == product.SupplierId);
            Category category = _context.Categories.FirstOrDefault(p => p.CategoryId == product.CategoryId);

            ViewBag.isDiscountActive = false;
            if (discount != null && discount.DiscountEndDate > DateTime.Now)
            {
                ViewBag.isDiscountActive = true;
            }
            IEnumerable<Product> ListProduct = _context.Products.Where(s => s.ProductActive == true).OrderByDescending(s => s.ProductId).ToList();
            ProductViewModels viewModel = new ProductViewModels
            {
                Product = product,
                ListProduct = ListProduct,
                DiscountName = discount?.DiscountName,
                DiscountPercent = discount.DiscountPercent,
                SupplierName = supplier?.SupplierName,
                CategoryName = category?.CategoryName,
                AddressSupplier = supplier?.SupplierAddress,
                DiscountPercentMap = _discount.GetDiscountPercentMap(ListProduct, _context.Discounts.ToList())
            };
            ViewBag.countCart = HttpContext.Session.GetInt32("counter");
            return View(viewModel);
        }

    }
}
