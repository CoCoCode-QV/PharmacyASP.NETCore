using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pharmacy.Models;
using Pharmacy.ViewsModels;
using System.Drawing.Printing;
using X.PagedList;

namespace Pharmacy.Controllers
{
  
    public class ProductsController : Controller
    {
        private readonly ProductModels _ProductModels;
        private readonly QlpharmacyContext _context;
        private readonly DiscountModels _discountModels;


        public ProductsController(QlpharmacyContext context, ProductModels productModels, DiscountModels discountModels)
        {
            _context = context;
            _ProductModels = productModels;
            _discountModels = discountModels;
        }

        private const int ItemsPerPage = 9;
        public IActionResult Index(string search, int? page, string orderby, int? selectedCategories)
        {
            TempData["SearchTerm"] = search;
            var listProducts = _ProductModels.GetProductsActive(search);
            
            if (selectedCategories != null )
            {
                listProducts = _ProductModels.GetProductsByCategoryId(selectedCategories);
            }
            else
            {
                 selectedCategories = 0;
            }

            switch (orderby)
            {
                case "increase":
                    listProducts = listProducts.OrderBy(p => p.ProductPrice).ToList();
                    break;
                case "reduce":
                    listProducts = listProducts.OrderByDescending(p => p.ProductPrice).ToList();
                    break;
                default:
                    listProducts = listProducts.OrderByDescending(s => s.ProductId).ToList();
                    break;
            }
            var pageNumber = page ?? 1;

                var listProductsCost = listProducts
                                    .SelectMany(product => _context.ProductCosts
                                    .Where(pc => pc.ProductId == product.ProductId && pc.CostActive && pc.ProductInventory > 0 && product.ProductActive == true && pc.ProductExpiryDate > DateTime.Now).Include(pc => pc.ProductDiscounts)
                                    .Include(pc =>pc.Product))
                                    .ToList();
            var pagedList = listProductsCost.ToPagedList(pageNumber, ItemsPerPage);
            var listCategory = _context.Categories.ToList();

            var viewModel = new ProductListViewModel
            {
                ProductCost = pagedList,
                Categories = listCategory,
                DiscountPercentMap = _discountModels.GetDiscountPercentMap(pagedList, _context.Discounts.ToList()),
                SelectedCategories = selectedCategories,
                orderby = orderby
            };
            ViewBag.countCart = HttpContext.Session.GetInt32("counter");
            return View(viewModel);
        }    
    }
}
