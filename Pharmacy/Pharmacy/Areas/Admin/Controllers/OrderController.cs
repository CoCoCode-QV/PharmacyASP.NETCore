using Amazon.S3.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using Pharmacy.Models;
using X.PagedList;

namespace Pharmacy.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin, Staff, SuperAdmin")]
    public class OrderController : Controller
    {
        private readonly OrderModels _orderModels;

        public OrderController(OrderModels orderModels)
        {
            _orderModels = orderModels;
        }

        public const int Items_Per_Page = 3;
        public IActionResult Index(int? page, string tab = "noaccept")
        {
            ViewData["CurrentTab"] = tab;
            var ListOrder = _orderModels.GetListOrder(tab);
            var pageNumber = page ?? 1;
            var onePage = ListOrder.ToPagedList(pageNumber, Items_Per_Page);

            return View(onePage);
        }

        public IActionResult Detail(int id , string tab )
        {
            if(id == 0)
            {
                return View();
            }
            var lisrOrderDetail = _orderModels.GetListOrderDetailByOrderId(id);
            var cartTotalPrice = lisrOrderDetail.Sum(item => item.OrderDetailsTemporaryPrice);
            ViewBag.CartTotalPrice = cartTotalPrice;
            ViewBag.OrderId = id;
            ViewData["CurrentTab"] = tab;
            return View(lisrOrderDetail);
        }

        public async Task<IActionResult> AcceptOrderAsync(int OrderId)
        {
            await _orderModels.AcceptOrder(OrderId);
            return RedirectToAction("Index", new { tab = "noaccept" });
        }

        public async Task<IActionResult> OrderDevlivery(int OrderId)
        {
            await _orderModels.OrderDelivery(OrderId);
            return RedirectToAction("Index", new { tab = "nodelivery" });
        }

        public async Task<IActionResult> DeleteOrder(int id, string reason)
        {
             await _orderModels.DeleteOrderAsync(id, reason);
            return RedirectToAction("Index", new { tab = "noaccept" });
        }
    }
}
