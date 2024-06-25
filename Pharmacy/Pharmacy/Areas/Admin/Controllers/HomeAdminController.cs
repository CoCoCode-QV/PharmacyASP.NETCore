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
using Stripe;
using System.Globalization;
using X.PagedList;

namespace Pharmacy.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin, Staff, SuperAdmin")]
    public class HomeAdminController : Controller
    {
        private readonly QlpharmacyContext _context;
        private readonly ProductCostModel _ProductCostModels;
        private readonly CustomerModels _customerModels;

        public HomeAdminController(QlpharmacyContext context, ProductCostModel productCostModels, CustomerModels customerModels)
        {
            _context = context;
            _ProductCostModels = productCostModels;
            _customerModels = customerModels;
        }

        [HttpGet]
        [Authorize(Roles = "Admin, SuperAdmin")]
        public IActionResult Index(string fromdate = null, string todate = null)
        {

            DateTime startDate;
            DateTime endDate;

            if (string.IsNullOrEmpty(fromdate) || string.IsNullOrEmpty(todate))
            {
                endDate = DateTime.Now.Date;
                startDate = endDate.AddMonths(-1);
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

        public const int Items_Per_Page = 10;
        public IActionResult statisticalInventory(string search, string condition, int? page, string orderby)
        {

            var listProductsCost = _ProductCostModels.productCostInventory(search, condition);

            var pageNumber = page ?? 1;
            switch (orderby)
            {
                case "increase":
                    listProductsCost = listProductsCost.OrderBy(p => p.ProductInventory).ToList();
                    break;
                case "reduce":
                    listProductsCost = listProductsCost.OrderByDescending(p => p.ProductInventory).ToList();
                    break;
                default:
                    listProductsCost = listProductsCost.OrderByDescending(s => s.ProductInventory).ToList();
                    break;
            }

            var onePage = listProductsCost.ToPagedList(pageNumber, Items_Per_Page);
            ViewBag.orderby = orderby;
            return View(onePage);
        
        }

        public IActionResult ChatAdmin()
        {
            return View("ChatAdmin", "HomeAdmin");
        }

        public IActionResult statisticalHistoryCustomer(string search, string condition, int? page)
        {
            var listCustomer = _customerModels.FetchHistoryPurchaseCustomer(search, condition);

            var pageNumber = page ?? 1;

            var onePage = listCustomer.ToPagedList(pageNumber, Items_Per_Page);

            return View(onePage);
          
        }
        public IActionResult TopSaleCustomer(int customerId, int? page)
        {
            var listCustomer = _customerModels.topSaleCustomer(customerId);

            var pageNumber = page ?? 1;

            var onePage = listCustomer.ToPagedList(pageNumber, Items_Per_Page);

            return View(onePage);

        }

        public IActionResult ProductCostExpiryDate(int? page)
        {
            var listProductsCost = _ProductCostModels.productCostExpiry();

            var pageNumber = page ?? 1;

            var onePage = listProductsCost.ToPagedList(pageNumber, Items_Per_Page);

            return View(onePage);

        }
    }
}
