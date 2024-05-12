using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
            ProductCost? productcost = _context.ProductCosts.Where(s => s.CostId == id).Include(s =>s.Product).FirstOrDefault() ;
          
            Supplier? supplier = _context.ProductCosts
                           .Where(pc => pc.ProductId == productcost.ProductId && pc.CostActive)
                           .Select(pc => pc.Supplier)
                           .SingleOrDefault();
            Category? category = _context.Categories.FirstOrDefault(p => p.CategoryId == productcost.Product.CategoryId);

          
            IEnumerable<ProductCost> listProductsCost = _context.ProductCosts.Where(s => s.CostActive).Include(pc =>pc.ProductDiscounts).Include(pc =>pc.Product).OrderByDescending(s => s.ProductId).ToList();

            ProductViewModels viewModel = new ProductViewModels
            {
                ProductCost = productcost,
                ListProductCost = listProductsCost,
             
                SupplierName = supplier?.SupplierName,
                CategoryName = category?.CategoryName,
                AddressSupplier = supplier?.SupplierAddress,
                DiscountPercentMap = _discount.GetDiscountPercentMap(listProductsCost, _context.Discounts.ToList())
            };
            ViewBag.countCart = HttpContext.Session.GetInt32("counter");
            return View(viewModel);
        }

    }
}
