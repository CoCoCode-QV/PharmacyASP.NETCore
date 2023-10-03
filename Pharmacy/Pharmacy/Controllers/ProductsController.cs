using Microsoft.AspNetCore.Mvc;
using Pharmacy.Data;
using Pharmacy.Models;
using Pharmacy.ViewsModels;
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

        private const int ItemsPerPage = 3;

        public IActionResult Index(string search, int? page)
        {
            var listProducts = _ProductModels.GetProductsActive(search);
            var pageNumber = page ?? 1;

            var pagedList = listProducts.ToPagedList(pageNumber, ItemsPerPage);

            var viewModel = new ProductListViewModel
            {
                Products = pagedList,
                DiscountPercentMap = _discountModels.GetDiscountPercentMap(listProducts, _context.Discounts.ToList())
            };

            return View(viewModel);
        }
    }
}
