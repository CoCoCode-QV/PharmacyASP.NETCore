using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pharmacy.Models;

namespace Pharmacy.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class OrderController : Controller
    {
        //private readonly OrderModels _orderModels;

        //public OrderController(OrderModels orderModels)
        //{
        //    _orderModels = orderModels;
        //}

        //public const int Items_Per_Page = 10;
        //public IActionResult Index(string search, int? page)
        //{
        //    var listDiscount = _orderModels.GetDiscount(search);

        //    var pageNumber = page ?? 1;

        //    var onePage = listDiscount.ToPagedList(pageNumber, Items_Per_Page);

        //    return View(onePage);
        //}

    }
}
