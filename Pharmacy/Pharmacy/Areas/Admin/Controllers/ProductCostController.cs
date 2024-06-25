using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Build.Logging;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

using Pharmacy.Models;
using Stripe;
using X.PagedList;

namespace Pharmacy.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin, Staff, SuperAdmin")]
    public class ProductCostController : Controller
    {
        private readonly ProductCostModel _ProductCostModels;
        private readonly QlpharmacyContext _context;

        public ProductCostController(ProductCostModel productCostModels, QlpharmacyContext context)
        {
            _ProductCostModels = productCostModels;
            _context = context;
        }

        public const int Items_Per_Page = 10;
        public IActionResult Index(string search,string condition, int? page)
        {
    
            var listProductsCost = _ProductCostModels.GetProductsCost(search, condition);

            var pageNumber = page ?? 1;

            var onePage = listProductsCost.ToPagedList(pageNumber, Items_Per_Page);

            return View(onePage) ;
        }

        public IActionResult Create()
        {
            getAll();
            return View();
        }
        public void getAll()
        {
            List<Supplier> suppliers = _context.Suppliers.ToList();
            List<Product> products = _context.Products.ToList();

            ViewBag.Suppliers = new SelectList(suppliers, "SupplierId", "SupplierName");
            ViewBag.Products = new SelectList(products, "ProductId", "ProductName");
        }


        [HttpPost]
        public async Task<IActionResult> Create(ProductCost item)
        {
            var product = _context.Products.Where(pc => pc.ProductId == item.ProductId).FirstOrDefault();
            if (item.CostPrice == 0 || item.ProductId == 0 || item.SupplierId == 0)
            {
                TempData["error"] = "Vui lòng điền đầy đủ thông tin.";
                return RedirectToAction("Create");
            }
            DateTime ProductExpiryDateDate = (DateTime)item.ProductExpiryDate!;
            DateTime ReceivingDate = (DateTime)item.ReceivingDate!;
            DateTime currentDate = DateTime.Now.Date;
            if (ProductExpiryDateDate < currentDate)
            {
                TempData["error"] = "Hạn sử dụng không hợp lệ";
                return RedirectToAction("Create");
            }
            if (ReceivingDate > currentDate)
            {
                TempData["error"] = "Thời gian nhập hàng không hợp lệ";
                return RedirectToAction("Create");
            }
            if(item.CostPrice < 0 || product.ProductPrice < item.CostPrice)
            {
                TempData["error"] = "Gía bán không hợp lệ";
                return RedirectToAction("Create");
            }
            await _ProductCostModels.CreateProductCost(item);
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Delete(int id)
        {
            var productCost = _ProductCostModels.GetProductCostById(id);

            if (productCost == null)
            {
                return NotFound();
            }

            if (!productCost.CostActive)
            {
                var orderdetail = _context.OrderDetails.Where(pc => pc.CostId == productCost.CostId);
                if(orderdetail.Any())
                {
                    // Trả về một thông báo rằng người dùng cần tạo một giá sản phẩm mới hoặc bật CostActive cũ của sản phẩm
                    TempData["ErrorMessage"] = "Sản phẩm này đã được bán nên bạn không thể xóa sản phẩm để tránh bị mất mát dữ liệu.";
                    return RedirectToAction("Index");
                }

                await _ProductCostModels.DeleteProductCost(id);
                return RedirectToAction("Index");
            }
            else
            {
                // Trả về một thông báo rằng người dùng cần tạo một giá sản phẩm mới hoặc bật CostActive cũ của sản phẩm
                TempData["ErrorMessage"] = "Bạn cần tạo một giá sản phẩm mới hoặc bật trạng thái giá cũ của sản phẩm.";
                return RedirectToAction("Index");
            }
        }

        public IActionResult Edit(int id)
        {
            getAll();
            return View(_ProductCostModels.GetProductCostById(id));
        }

        [HttpPost]
        public async Task<IActionResult> EditAsync(ProductCost item)
        {
            if(item == null)
            {
                return View();
            }
            else
            {
                 var product = _context.Products.Where(pc => pc.ProductId == item.ProductId).FirstOrDefault();
                DateTime ProductExpiryDateDate = (DateTime)item.ProductExpiryDate!;
                DateTime ReceivingDate = (DateTime)item.ReceivingDate!;
                DateTime currentDate = DateTime.Now.Date;
                if (ProductExpiryDateDate < currentDate)
                {
                    TempData["error"] = "Hạn sử dụng không hợp lệ";
                    return RedirectToAction("Edit", new { id = item.CostId });
                }
                if (ReceivingDate > currentDate)
                {
                    TempData["error"] = "Thời gian nhập hàng không hợp lệ";
                    return RedirectToAction("Edit", new { id = item.CostId });
                }
                if (item.CostPrice < 0 || product.ProductPrice < item.CostPrice)
                {
                    TempData["error"] = "Gía bán không hợp lệ";
                    return RedirectToAction("Edit", new { id = item.CostId });
                }
                if (item.CostActive == false)
                {
                    var otherProductCosts = _context.ProductCosts.Where(pc => pc.ProductId == item.ProductId && pc.CostId != item.CostId);
                    bool allInactive = otherProductCosts.All(pc => !pc.CostActive);
                    //kiểm tra tất cả giá sản phẩm đều là false thì lỗi
                    if(allInactive)
                    {
                        TempData["error"] = "Vui lòng tạo giá sản phẩm mới cho sản phẩm trước khi tắt trạng thái giá";
                        return RedirectToAction("Edit", new { id = item.CostId });
                    }     
                }

                await _ProductCostModels.Edit(item);
                return RedirectToAction("Index");
            }
        }
    }
}
