using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pharmacy.Data;
using Pharmacy.Models;
using Pharmacy.ViewsModels;
using X.PagedList;

namespace Pharmacy.Controllers
{
    public class PrescriptionController : Controller
    {

        private readonly ProductModels _ProductModels;
        private readonly QlpharmacyContext _context;
        private readonly DiscountModels _discountModels;


        public PrescriptionController(QlpharmacyContext context, ProductModels productModels, DiscountModels discountModels)
        {
            _context = context;
            _ProductModels = productModels;
            _discountModels = discountModels;
        }

        private const int ItemsPerPage = 12;
        public IActionResult Index(int? page, string orderby)
        {
            var listProducts = _ProductModels.GetProductsPresciption();

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

            var pagedList = listProducts.ToPagedList(pageNumber, ItemsPerPage);
            var viewModel = new ProductListViewModel
            {
                Products = pagedList,
                DiscountPercentMap = _discountModels.GetDiscountPercentMap(listProducts, _context.Discounts.ToList()),
                orderby = orderby
            };
            ViewBag.countCart = HttpContext.Session.GetInt32("counter");
            return View(viewModel);
        
        }
    }
}
