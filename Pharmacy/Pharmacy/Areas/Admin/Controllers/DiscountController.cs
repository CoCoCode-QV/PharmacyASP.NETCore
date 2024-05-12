using Microsoft.AspNetCore.Mvc;
using Pharmacy.Models;

using X.PagedList;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Pharmacy.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin, Staff")]
    public class DiscountController : Controller
    {
        private readonly DiscountModels _discountModels;
        private readonly QlpharmacyContext _context;

        public DiscountController(DiscountModels discountModels , QlpharmacyContext context)
        {
            _discountModels = discountModels;
            _context = context;
        }

        public const int Items_Per_Page = 10;
        public IActionResult Index(string search, int? page)
        {
            var listDiscount = _discountModels.GetDiscount(search);

            var pageNumber = page ?? 1;

            var onePage = listDiscount.ToPagedList(pageNumber, Items_Per_Page);

            return View(onePage);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateAsync(Discount discount)
        {
            if (discount.DiscountName == null || discount.DiscountPercent == null)
            {
                return View();
            }
            else
            {
                //DateTime startDate = (DateTime)discount.DiscountStartDate;
                //DateTime endDate = (DateTime)discount.DiscountEndDate;
                //DateTime currentDate = DateTime.Now.Date;
                //if (startDate > endDate || startDate < currentDate || endDate < currentDate)
                //{
                //    TempData["error"] = "Ngày bắt đầu khuyến mãi và kết thúc không hợp lệ";
                //    return View();
                //}
                await _discountModels.CreatDiscountAsync(discount);
                return RedirectToAction("Index");
            }
        }

        public async Task<IActionResult>  Delete(int id)
        {
            await  _discountModels.DeleteDiscountAsync(id);
            return RedirectToAction("Index");
        }

        public IActionResult Edit(int id)
        {

            return View(_discountModels.GetDiscount(id));
        }

        [HttpPost]
        public async Task<IActionResult> EditAsync(Discount discount)
        {
            if (discount.DiscountName == null || discount.DiscountPercent == null)
            {

                return View();
            }
            else
            {
                //DateTime startDate = (DateTime)discount.DiscountStartDate;
                //DateTime endDate = (DateTime)discount.DiscountEndDate;
                //DateTime currentDate = DateTime.Now.Date;
                //if (startDate > endDate || startDate < currentDate || endDate < currentDate)
                //{
                //    TempData["error"] = "Ngày bắt đầu khuyến mãi và kết thúc không hợp lệ";
                //    return View();
                //}
                await _discountModels.EditDiscount(discount);
                return RedirectToAction("Index");
            }
        }


        #region ProductDiscount
        public IActionResult ProductDiscount(string search, int? page)
        {
            var listDiscount = _discountModels.GetProductDiscount(search);

            var pageNumber = page ?? 1;

            var onePage = listDiscount.ToPagedList(pageNumber, Items_Per_Page);

            return View(onePage);
        }

        public IActionResult CreateProductDiscount()
        { 
            
            getItemDiscountProduct();
            return View();
          
        }

        public void getItemDiscountProduct()
        {

            var productList = new List<SelectListItem>();
            var discountList = new List<SelectListItem>();


            var ProductCosts = _context.ProductCosts.ToList();
            var discounts = _context.Discounts.ToList();
            if (ProductCosts.Count > 0 && discounts.Count > 0)
            {
                discountList.AddRange(discounts.Select(d => new SelectListItem { Value = d.DiscountId.ToString(), Text = d.DiscountName }));
                ViewBag.DiscountName = new SelectList(discountList, "Value", "Text");
                foreach (var item in ProductCosts)
                {
                    // Lấy thông tin sản phẩm tương ứng với CostId hiện tại
                    var productName = _context.Products.FirstOrDefault(p => p.ProductId == item.ProductId)?.ProductName;

                    // Lấy thông tin nhà cung cấp tương ứng với CostId hiện tại
                    var supplierName = _context.Suppliers.FirstOrDefault(s => s.SupplierId == item.SupplierId)?.SupplierName;

                    // Thêm vào danh sách
                    productList.Add(new SelectListItem { Value = item.CostId.ToString(), Text = $"{productName} - {supplierName}" });
                    // Đặt danh sách sản phẩm vào ViewBag
                    ViewBag.ProductCost = new SelectList(productList, "Value", "Text");
                   
                }
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateProductDiscountAsync(ProductDiscount item)
        {
            if (item.CostId == null || item.DiscountId == null)
            {
                return View();
            }
            else
            {
                DateTime startDate = (DateTime)item.DiscountStartDate!;
                DateTime endDate = (DateTime)item.DiscountEndDate!;
                DateTime currentDate = DateTime.Now.Date;
                if (startDate > endDate || startDate < currentDate || endDate < currentDate)
                {
                    TempData["error"] = "Ngày bắt đầu khuyến mãi và kết thúc không hợp lệ";
                    return View();
                }
                await _discountModels.CreatProductDiscountAsync(item);
                return RedirectToAction("ProductDiscount");
            }
        }

        public async Task<IActionResult> DeleteProductDiscount(int id)
        {
            
            await _discountModels.DeleteDiscountProductAsync(id);
            return RedirectToAction("ProductDiscount");
        }

        public IActionResult EditProductDiscount(int id)
        {
            getItemDiscountProduct();
            var data = _discountModels.GetDiscountProduct(id);
            if(data == null)
            {
                return View("ProductDiscount");
            }
            return View(data);
        }

        [HttpPost]
        public async Task<IActionResult> EditProductDiscount(ProductDiscount item)
        {
            if (item.CostId == null || item.DiscountId == null)
            {
                return View();
            }
            else
            {
                DateTime startDate = (DateTime)item.DiscountStartDate!;
                DateTime endDate = (DateTime)item.DiscountEndDate!;
                DateTime currentDate = DateTime.Now.Date;
                if (startDate > endDate || startDate < currentDate || endDate < currentDate)
                {
                    TempData["error"] = "Ngày bắt đầu khuyến mãi và kết thúc không hợp lệ";
                    return View();
                }
                await _discountModels.EditDiscount(item);
                return RedirectToAction("ProductDiscount");
            }
        }

        #endregion
    }
}
