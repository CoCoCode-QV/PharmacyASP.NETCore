using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using Pharmacy.Models;
using X.PagedList;

namespace Pharmacy.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin, Staff, SuperAdmin")]
    public class CustomerController : Controller
    {
        private readonly CustomerModels _customerModels;
        private OrderModels _orderModels;

        public CustomerController(CustomerModels customerModels, OrderModels orderModels)
        {
            _customerModels = customerModels;
            _orderModels = orderModels;
        }

        public const int Items_Per_Page = 10;
        public IActionResult Index(string search, string condition, int? page)
        {
            var listSupplier = _customerModels.getListCustomer(search, condition);

            var pageNumber = page ?? 1;

            var onePageSupplier = listSupplier.ToPagedList(pageNumber, Items_Per_Page);

            return View(onePageSupplier);
        }
        public IActionResult CheckHistoryOrder(int id, int? page)
        {

            var HistoryPurchase = _customerModels.historyPurchase(id);
            var pageNumber = page ?? 1;
            var onePageSupplier = HistoryPurchase.ToPagedList(pageNumber, Items_Per_Page);
            ViewBag.CustomerId = id;
            return View(onePageSupplier);
        }
        public IActionResult CheckHistorypurchase(int id, int customerid)
        {
            if (id == 0)
            {
                return View();
            }
            var lisrOrderDetail = _orderModels.GetListOrderDetailByOrderId(id);
            var cartTotalPrice = lisrOrderDetail.Sum(item => item.OrderDetailsTemporaryPrice);
            ViewBag.customerid = customerid;
            ViewBag.CartTotalPrice = cartTotalPrice;
            return View(lisrOrderDetail);
        }
    }
}
