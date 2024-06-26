using MailKit.Search;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using Pharmacy.Models;
using Pharmacy.ViewsModels;
using System.Security.Claims;
using System.Text;
using X.PagedList;


namespace Pharmacy.Controllers
{
    public class CartController : Controller
    {
        private readonly QlpharmacyContext _context;
        private readonly CustomerModels _customer;
        private readonly CartModels _cart;
        private readonly OrderModels _order;
        private readonly ProductCostModel _productCost;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CartController(QlpharmacyContext context, CustomerModels customerModels, ProductCostModel productCost,CartModels cartModels,OrderModels order, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _customer = customerModels;
            _cart = cartModels;
            _httpContextAccessor = httpContextAccessor;
            _order = order;
            _productCost = productCost;
        }
        public async Task<IActionResult> IndexAsync()
        {
            var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
                return View();
            var CustomerInfo = _customer.GetCustomer(userId);
            var Cart = _cart.getCartByCustomerId(CustomerInfo.CustomerId);


            var cartItems = _context.CartDetails
                      .Where(cd => cd.CartId == Cart.CartId)
                        .Include(cd => cd.Cost)
                      .Select(cd => new CartItemViewModels
                      {
                          CartDetailId = cd.CartDetailId,
                          CostId = cd.CostId,
                          ProductId = cd.Cost.ProductId,
                          ProductName = cd.Cost.Product.ProductName!,
                          ProductImage = cd.Cost.Product.ProductImage!,
                          CartDetailPriceCurrent = cd.CartDetailPriceCurrent,
                          CartDetailQuantity = cd.CartDetailQuantity,
                          CartDetailTemporaryPrice = cd.CartDetailTemporaryPrice
                      })
                      .ToList();

            var cartTotalPrice = cartItems.Sum(item => item.CartDetailTemporaryPrice);

            ViewBag.CartTotalPrice = cartTotalPrice;
            ViewBag.CustomerId = CustomerInfo.CustomerId;
            //count
            int countCart = _cart.TotalQuantityCartDetail(Cart.CartId);
            _httpContextAccessor.HttpContext.Session.SetInt32("counter", countCart);
            ViewBag.countCart = _httpContextAccessor.HttpContext.Session.GetInt32("counter");
            return View(cartItems);

        }
        [HttpPost]
        public async Task<IActionResult> AddToCart(double? Price, int CostId, int quantity = 1)
        {
            var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
                return RedirectToAction("Index", "Login");
            var CustomerInfo = _customer.GetCustomer(userId);
            var Cart = _cart.getCartByCustomerId(CustomerInfo.CustomerId);

            var existingCartDetail = _context.CartDetails
                 .FirstOrDefault(cd => cd.CartId == Cart.CartId && cd.CostId == CostId);

            var productCost = _context.ProductCosts
                .Include(p => p.ProductDiscounts).ThenInclude(d=>d.Discount)
                .FirstOrDefault(p => p.CostId == CostId);
        
            if (productCost == null)
            {
                return NotFound();
            }
            if (existingCartDetail != null)
            {
                // Nếu sản phẩm đã tồn tại trong giỏ hàng, cập nhật số lượng và tính lại giá tạm thời
                existingCartDetail.CartDetailQuantity += quantity;
                existingCartDetail.CartDetailTemporaryPrice = existingCartDetail.CartDetailQuantity * Price;
                await _cart.UpdateCartDetailAsync(existingCartDetail);
            }
            else
            {
                // Tính tổng giá tạm thời của các sản phẩm
                double? totalTemporaryPrice = Price * quantity;

                var cartDetail = new CartDetail
                {
                    CartId = Cart.CartId,
                    CostId = productCost.CostId,
                    CartDetailQuantity = quantity,
                    CartDetailPriceCurrent = Price,
                    CartDetailTemporaryPrice = totalTemporaryPrice
                };
                await _cart.CreateCartDetail(cartDetail);
            }
            return RedirectToAction("Index");
        }
        [HttpPost]
        public async Task<IActionResult> UpdateToCart(double? Price,int CostId, int quantity = 1)
        {
            var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
                return View();
            var CustomerInfo = _customer.GetCustomer(userId);
            var Cart = _cart.getCartByCustomerId(CustomerInfo.CustomerId);

            var existingCartDetail = _context.CartDetails
                 .FirstOrDefault(cd => cd.CartId == Cart.CartId && cd.CostId == CostId);
            if(quantity <= 0)
            {
				await _cart.DeleteDetailAsync(existingCartDetail.CartDetailId);
				return RedirectToAction("Index");
			}
            if (existingCartDetail != null)
            {
               bool ischeck = _productCost.CheckInventory(CostId, quantity);
                if(ischeck)
                {
                    // Nếu sản phẩm đã tồn tại trong giỏ hàng, cập nhật số lượng và tính lại giá tạm thời
                    existingCartDetail.CartDetailQuantity = quantity;
                    existingCartDetail.CartDetailTemporaryPrice = existingCartDetail.CartDetailQuantity * Price;
                    await _cart.UpdateCartDetailAsync(existingCartDetail);
                }
                else
                {
                    TempData["ErrorCheck"] = "Sản phẩm đã hết trong kho vui lòng không tăng thêm số lượng sản phẩm";
                    return RedirectToAction("Index");
                }
            }
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {

            await _cart.DeleteDetailAsync(id);
            return RedirectToAction("Index");
        }

        public const int Items_Per_Page = 3;
        public async Task<IActionResult> HistoryOrder(int? page, string tab = "noaccept")
        {
            ViewData["CurrentTab"] = tab;
            var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
                return RedirectToAction("Index", "Login");
            var CustomerInfo = _customer.GetCustomer(userId);

            var ListOrder = _order.GetListOrderByCustomerId(CustomerInfo.CustomerId, tab);

            var pageNumber = page ?? 1;

            var onePage = ListOrder.ToPagedList(pageNumber, Items_Per_Page);
           
            return View(onePage);
        }

        public IActionResult OrderDetail(int id)
        {
            if (id == 0)
            {
                return View();
            }
            var lisrOrderDetail = _order.GetListOrderDetailByOrderId(id);
            var cartTotalPrice = lisrOrderDetail.Sum(item => item.OrderDetailsTemporaryPrice);
            ViewBag.CartTotalPrice = cartTotalPrice;
            ViewBag.OrderId = id;
            return View(lisrOrderDetail);
        }


        public  IActionResult DeletedOrder(int orderId)
        {
            var order = _context.Orders
                                .Include(o => o.OrderDetails).ThenInclude(od => od.Cost).ThenInclude(x=> x.Product).Include(o => o.Customer)
                                .SingleOrDefault(o => o.OrderId == orderId && !o.OrderAccept);

            if (order != null)
            {
                var orderDetailsList = order.OrderDetails.ToList();
                SendMailService sendMailService = new SendMailService();
                string emailBody = $@"<html>
                <head></head>
                <body>
					    <p>Bạn đã hủy đơn hàng:</p>
					    {GenerateOrderDetailTable(orderDetailsList)}
                        
                        <em>Chúng tôi sẽ hoàn tiền lại cho quý khách đã thanh toán sau mail này, nếu quý khách chưa nhận được tiền vui lòng liên hệ lại cho chúng tôi</em>
                               
                </body>
                </html>";
                sendMailService.SendMail(order.Customer.CustomerEmail, "VTPharmacy hủy đơn hàng", emailBody, "");
                order.OrderStatus = -1;

                //_context.OrderDetails.RemoveRange(order.OrderDetails);
                //_context.Orders.Remove(order);
                _context.SaveChanges();
                TempData["SuccessMessage"] = "Huỷ đơn hàng thành công, bạn vui lòng kiểm tra mail để nhận lại tiền đã thanh toán";
                return RedirectToAction("HistoryOrder", TempData["SuccessMessage"]);
            }
            TempData["ErrorMessage"] = "Hủy đơn hàng thất bại, không tìm thấy đơn hàng";
            return RedirectToAction("HistoryOrder", TempData["ErrorMessage"]);
        }
        public string GenerateOrderDetailTable(List<OrderDetail> orderDetails)
        {

            StringBuilder htmlTable = new StringBuilder();
            htmlTable.Append("<table style='width:100%;border-collapse: collapse;' border='1'><tr><th style='text-align:left;padding: 10px;'>Tên sản phẩm</th><th style='text-align:center;padding: 10px;'>Giá (VNĐ)</th><th style='text-align:center;padding: 10px;'>Số lượng</th><th style='text-align:center;padding: 10px;'>Tổng tiền(VNĐ)</th></tr>");

            foreach (var item in orderDetails)
            {
                htmlTable.Append("<tr>");
                htmlTable.Append("<td style='text-align:left;padding: 10px;'>" + item.Cost.Product.ProductName + "</td>");
                htmlTable.Append("<td style='text-align:center;padding: 10px;'>" + string.Format("{0:N0} VNĐ", item.OrderDetailsPrice) + "</td>");
                htmlTable.Append("<td style='text-align:center;padding: 10px;'>" + item.OrderDetailsQuantity + "</td>");
                htmlTable.Append("<td style='text-align:center;padding: 10px;'>" + string.Format("{0:N0} VNĐ", item.OrderDetailsTemporaryPrice) + "</td>");
                htmlTable.Append("</tr>");
            }
            // Tính tổng tiền
            var TotalPrice = orderDetails.Sum(item => item.OrderDetailsTemporaryPrice);
            htmlTable.Append("<tr><td colspan='3' style='text-align:right;padding: 10px;'>Tổng cộng:</td><td style='text-align:center;padding: 10px;'>" + string.Format("{0:N0} VNĐ", TotalPrice) + "</td></tr>");

            htmlTable.Append("</table>");

            return htmlTable.ToString();
        }


    }
}