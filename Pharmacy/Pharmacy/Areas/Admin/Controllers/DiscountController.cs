using Microsoft.AspNetCore.Mvc;
using Pharmacy.Models;
using Pharmacy.Data;
using X.PagedList;
using Microsoft.AspNetCore.Authorization;

namespace Pharmacy.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class DiscountController : Controller
    {
        private readonly DiscountModels _discountModels;

        public DiscountController(DiscountModels discountModels)
        {
            _discountModels = discountModels;
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
            if (discount.DiscountName == null || discount.DiscountPercent == null|| discount.DiscountStartDate == null || discount.DiscountEndDate == null)
            {
                return View();
            }
            else
            {
                DateTime startDate = (DateTime)discount.DiscountStartDate;
                DateTime endDate = (DateTime)discount.DiscountEndDate;
                DateTime currentDate = DateTime.Now.Date;
                if (startDate > endDate || startDate < currentDate || endDate < currentDate)
                {
                    TempData["error"] = "Ngày bắt đầu khuyến mãi và kết thúc không hợp lệ";
                    return View();
                }
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
            if (discount.DiscountName == null || discount.DiscountPercent == null || discount.DiscountStartDate == null || discount.DiscountEndDate == null)
            {

                return View();
            }
            else
            {
                DateTime startDate = (DateTime)discount.DiscountStartDate;
                DateTime endDate = (DateTime)discount.DiscountEndDate;
                DateTime currentDate = DateTime.Now.Date;
                if (startDate > endDate || startDate < currentDate || endDate < currentDate)
                {
                    TempData["error"] = "Ngày bắt đầu khuyến mãi và kết thúc không hợp lệ";
                    return View();
                }
                await _discountModels.EditDiscount(discount);
                return RedirectToAction("Index");
            }
        }
    }
}
