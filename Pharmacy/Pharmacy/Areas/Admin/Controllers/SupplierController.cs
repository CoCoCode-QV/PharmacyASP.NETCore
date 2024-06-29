using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using Pharmacy.Models;
using X.PagedList;

namespace Pharmacy.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin, Staff, SuperAdmin")]
    public class SupplierController : Controller
    {
        private readonly SupplierModels _supplierModels;
        private readonly QlpharmacyContext _context;
        public SupplierController(SupplierModels supplierModels, QlpharmacyContext context)
        {
            this._supplierModels = supplierModels;
            _context = context;
        }

        public const int Items_Per_Page = 10;
        public IActionResult Index(string search,string condition,int? page)
        {
            var listSupplier = _supplierModels.GetSuppliers(search,condition);

            var pageNumber = page ?? 1;

            var onePageSupplier = listSupplier.ToPagedList(pageNumber, Items_Per_Page);

            return View(onePageSupplier);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public  IActionResult Create(Supplier supplier)
        {
            if (supplier.SupplierName == null || supplier.SupplierAddress == null || supplier.SupplierEmail == null || supplier.SupplierPhone == null)
            {
                TempData["error"] = "Vui lòng điền đầy đủ thông tin.";
                return View();
            }
            else
            {
                 _supplierModels.CreatSupplier(supplier);
                TempData["Success"] = "Thêm nhà cung cấp thành công.";

                return RedirectToAction("Index");
            }
        }

        public  IActionResult Delete(int id)
        {
            Supplier supplier = _supplierModels.GetSupplier(id);
            if(supplier != null)
            {
                List<ProductCost> productCosts = _context.ProductCosts.ToList();
                foreach (var item in productCosts)
                {
                    if(item.SupplierId == supplier.SupplierId)
                    {
                        TempData["Error"] = "Nhà cung cấp đã có trong dữ liệu của giá nhập sản phẩm không thể xóa.";
                        return RedirectToAction("Index");
                    }
                }
                _supplierModels.DeleteSupplier(id);
                TempData["Success"] = "Xóa nhà cung cấp thành công.";
                return RedirectToAction("Index");
            }
            TempData["Error"] = "Không tìm thấy nhà cung cấp.";
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
                TempData["Error"] = "Vui lòng điền đầy đủ thông tin.";

                return View();
            }
            else
            {
                _supplierModels.EditSupplierAsync(supplier);
                TempData["Success"] = "Sửa nhà cung cấp thành công.";
                return RedirectToAction("Index");
            }
        }
    }
}
