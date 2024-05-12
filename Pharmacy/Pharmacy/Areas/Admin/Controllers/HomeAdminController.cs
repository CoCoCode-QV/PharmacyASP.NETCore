using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using NuGet.Versioning;

using Pharmacy.Models;
using Pharmacy.ViewsModels;
using System.Globalization;

namespace Pharmacy.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin, Staff")]
    public class HomeAdminController : Controller
    {
        private readonly QlpharmacyContext _context;

        public HomeAdminController(QlpharmacyContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult Index(string fromdate = null, string todate = null)
        {

            DateTime startDate;
            DateTime endDate;

            if (string.IsNullOrEmpty(fromdate) || string.IsNullOrEmpty(todate))
            {
                endDate = DateTime.Now.Date;
                startDate = endDate.AddMonths(-1).AddDays(1 - endDate.Day);
                ViewBag.fromdate = startDate;
                ViewBag.todate = endDate;
            }
            else
            {
                // Nếu có giá trị fromdate hoặc todate, chuyển đổi chúng thành định dạng DateTime
                startDate = DateTime.ParseExact(fromdate, "yyyy-MM-dd", null, DateTimeStyles.None);
                endDate = DateTime.ParseExact(todate, "yyyy-MM-dd", null, DateTimeStyles.None);
                ViewBag.fromdate = startDate;
                ViewBag.todate = endDate;
            }
            //var Revennue = from o in _context.Orders
            //               join od in _context.OrderDetails
            //               on o.OrderId equals od.OrderId
            //               join p in _context.ProductCosts
            //               on od.CostId equals p.CostId
            //               where o.OrderStatus == 1 &&  o.OrderDelivery == 1 && o.OrderDate.Date >= startDate.Date && o.OrderDate.Date <= endDate.Date 
            //               select new
            //               {
            //                   OrderDate = o.OrderDate,
            //                   OrderDetailsTemporaryPrice = od.OrderDetailsTemporaryPrice,
            //                   ProductName = p.Product.ProductName,
            //                   DataQuantitySell = od.OrderDetailsQuantity
            //               };
            var Revennue = _context.Orders
                      .Join(_context.OrderDetails, o => o.OrderId, od => od.OrderId, (o, od) => new { Order = o, OrderDetail = od })
                      .Join(_context.ProductCosts, joinResult => joinResult.OrderDetail.CostId, p => p.CostId, (joinResult, p) => new
                      {
                          Order = joinResult.Order,
                          OrderDetail = joinResult.OrderDetail,
                          ProductCost = p
                      })
                      .Where(joinResult => joinResult.Order.OrderStatus == 1 && joinResult.Order.OrderDelivery == 1 && joinResult.Order.OrderAccept && joinResult.Order.OrderDate.Date >= startDate.Date && joinResult.Order.OrderDate.Date <= endDate.Date)
                      .Select(joinResult => new
                      {
                          OrderDate = joinResult.Order.OrderDate,
                          OrderDetailsTemporaryPrice = joinResult.OrderDetail.OrderDetailsTemporaryPrice,
                          ProductName = joinResult.ProductCost.Product.ProductName,
                          DataQuantitySell = joinResult.OrderDetail.OrderDetailsQuantity
                      })
                      .ToList();



            var bestSellingsProducts = Revennue.GroupBy(o => o.ProductName)
                                                .Select(X => new ChartBestSellingsViewModel
                                                {
                                                    ProductName = X.Key,
                                                    DataQuantitySell = X.Sum(o => o.DataQuantitySell)
                                                }).OrderByDescending(X => X.DataQuantitySell)
                                                .ToList();

            var result = Revennue.GroupBy(x => x.OrderDate.Date)
                      .Select(x => new ChartRevenueViewModels
                      {
                          LabelsDate = x.Key,
                          DataRevenue = x.Sum(y => y.OrderDetailsTemporaryPrice)
                      })
                      .ToList();

            var chartDataViewModel = new chartDataViewModel
            {
                BestSellingProducts = bestSellingsProducts,
                Result = result
            };


            return View(chartDataViewModel);
           
        }


        public IActionResult ChatAdmin()
        {
            return View("ChatAdmin", "HomeAdmin");
        }
    }
}
