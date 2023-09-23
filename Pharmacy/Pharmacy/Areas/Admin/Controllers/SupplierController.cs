using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pharmacy.Models;
using X.PagedList;

namespace Pharmacy.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class SupplierController : Controller
    {
        private readonly SupplierModels _supplierModels;
        public SupplierController(SupplierModels supplierModels)
        {
           this._supplierModels = supplierModels;
        }

        public const int Items_Per_Page = 10;
        public IActionResult Index(string search, int? page)
        {
            var listSupplier = _supplierModels.GetSuppliers(search);

            var pageNumber = page ?? 1;

            var onePageSupplier = listSupplier.ToPagedList(pageNumber, Items_Per_Page);

            return View(onePageSupplier);
        }
    }
}
