using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pharmacy.Data;
using Pharmacy.Models;
using X.PagedList;

namespace Pharmacy.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class OrderController : Controller
    {
        private readonly OrderModels _orderModels;

        public OrderController(OrderModels orderModels)
        {
            _orderModels = orderModels;
        }

        public const int Items_Per_Page = 3;
        public IActionResult Index(int? page, string tab = "accept")
        {
            ViewData["CurrentTab"] = tab;
            var ListOrder = _orderModels.GetListOrder(tab);
            var pageNumber = page ?? 1;
            var onePage = ListOrder.ToPagedList(pageNumber, Items_Per_Page);

            return View(onePage);
        }

        public IActionResult Detail(int id)
        {
            if(id == 0)
            {
                return View();
            }
            var lisrOrderDetail = _orderModels.GetListOrderDetailByOrderId(id);
            var cartTotalPrice = lisrOrderDetail.Sum(item => item.OrderDetailsTemporaryPrice);
            ViewBag.CartTotalPrice = cartTotalPrice;
            ViewBag.OrderId = id;
            return View(lisrOrderDetail);
        }

        public async Task<IActionResult> AcceptOrderAsync(int OrderId)
        {
            await _orderModels.AcceptOrder(OrderId);
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> DeleteOrder(int id)
        {
            await _orderModels.DeleteOrderAsync(id);

            return RedirectToAction("Index");
        }
    }
}
