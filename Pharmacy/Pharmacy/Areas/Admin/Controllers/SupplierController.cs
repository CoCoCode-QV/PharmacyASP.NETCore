using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pharmacy.Data;
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

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Supplier supplier)
        {
            if (supplier.SupplierName == null || supplier.SupplierAddress == null || supplier.SupplierEmail == null || supplier.SupplierPhone == null)
            {
               
                return View();
            }
            else
            {
                _supplierModels.CreatSupplier(supplier);
                return RedirectToAction("Index");
            }
        }

        public IActionResult Delete(int id)
        {
            _supplierModels.DeleteSupplier(id);
            return RedirectToAction("Index");
        }

        public IActionResult Edit(int id)
        {
            return View(_supplierModels.GetSupplier(id));
        }

        [HttpPost]
        public IActionResult Edit(Supplier supplier)
        {
            if (supplier.SupplierName == null || supplier.SupplierAddress == null || supplier.SupplierEmail == null || supplier.SupplierPhone == null)
            {
                return View();
            }
            else
            {
                _supplierModels.EditSupplierAsync(supplier);
                return RedirectToAction("Index");
            }
        }
    }
}
