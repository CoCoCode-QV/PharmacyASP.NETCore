using Amazon.S3.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.CodeAnalysis.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

using Pharmacy.Models;
using Pharmacy.ViewsModels;
using Stripe;
using Stripe.Checkout;
using System.Globalization;

namespace Pharmacy.Controllers
{
    public class PaymentController : Controller
    {
		private readonly CartModels _cart;
		private readonly CustomerModels _customer;
		private readonly OrderModels _order;
		private readonly ProductModels _product;
		private readonly StripeSettings _stripeSettings;
		private readonly QlpharmacyContext _context;


		public PaymentController( CartModels cartModels, CustomerModels customer, OrderModels order, ProductModels product, StripeSettings stripesetting, QlpharmacyContext context)
		{
			_cart = cartModels;
			_customer = customer;
			_order = order;
			_product = product;
			_stripeSettings = stripesetting;
			_context = context;

        }
		[HttpPost]
        public IActionResult Index(int CustomerId, long? amount)
		{
			if (CustomerId == 0)
			{
				ViewBag.ErrorLogin = "Vui lòng đăng nhập để mua sản phẩm và thanh toán!";
				return View();
			}
			var customer = _customer.GetCustomerByid(CustomerId);
			var cart = _cart.getCartByCustomerId(CustomerId);
			var cartDetail = _cart.GetCartDetailsByCartId(cart.CartId);
			if(cartDetail.Count() == 0)
			{
				ViewBag.ErrorCartDetail = "Hiện chưa có sản phẩm nào để thanh toán!";
				return View(customer);
			}
			if(customer.CustomerAddress == null || customer.CustomerAddress == "" || customer.CustomerAddress == null || customer.CustomerAddress == "")
			{
				ViewBag.ErrorCustomer = "Vui lòng cập nhật thông tin khách hàng trước khi thanh toán!";
				return View(customer);
			}
			ViewBag.amount = amount;
			return View(customer);
		}

		
		public async Task<IActionResult> CreateOrderAsync(Customer customer)
		{
			// Kiểm tra xem có cartdetail nào cho người dùng này không
			var cart = _cart.getCartByCustomerId(customer.CustomerId);

			if (_cart.DoesCartHaveDetails(cart.CartId))
			{
				// Tạo một đơn đặt hàng mới
				var newOrder = new Order
				{
					OrderAddress = customer.CustomerAddress,
					OrderStatus = 1,
					CustomerId = customer.CustomerId,
					OrderDate = DateTime.Now,
					OrderAccept = false
				};

				await _order.CreateOrder(newOrder);

				// Lấy cartdetails của người dùng
				var cartDetails = _cart.GetCartDetailsByCartId(cart.CartId);

				List<CartItemViewModels> listCartItem = new List<CartItemViewModels>();
				// Di chuyển dữ liệu từ CartDetail sang OrderDetails
				foreach (var cartDetail in cartDetails)
				{
					var cost = _context.ProductCosts.Where(e => e.CostId == cartDetail.CostId).Include(e=>e.Product).FirstOrDefault();
					var ProductDiscount =  _context.ProductDiscounts.Where(e => e.CostId == cartDetail.CostId && e.DiscountEndDate > DateTime.Now && e.DiscountStartDate < DateTime.Now).FirstOrDefault();

                    var orderDetail = new OrderDetail
					{
						OrderId = newOrder.OrderId,
						CostId = cost.CostId!,
						OrderDiscountId = ProductDiscount == null ? 0 : ProductDiscount.ProductDiscountId,
						OrderDetailsQuantity = cartDetail.CartDetailQuantity,
						OrderDetailsPrice = cartDetail.CartDetailPriceCurrent,
						OrderDetailsTemporaryPrice = cartDetail.CartDetailTemporaryPrice
					};
					
					var cartItem = new CartItemViewModels {
					
						ProductName = cost.Product.ProductName!,
						ProductImage = cost.Product.ProductImage!,
						CartDetailPriceCurrent = cartDetail.CartDetailPriceCurrent,
						CartDetailQuantity = cartDetail.CartDetailQuantity,
						CartDetailTemporaryPrice = cartDetail.CartDetailTemporaryPrice

					};
					listCartItem.Add(cartItem);

					await _order.CreateOrderDetail(orderDetail);
					await _cart.DeleteDetailAsync(cartDetail.CartDetailId);
				}
				SendMailService sendMailService = new SendMailService();
				string emailBody = $@"<html>
                    <head></head>
                    <body>
						<p>Chi tiết đơn hàng của bạn:</p>
						{_cart.GenerateCartItemsTable(listCartItem)}
                    </body>
                    </html>";
				sendMailService.SendMail(customer.CustomerEmail, "Mua hàng VTPharmacy:", emailBody, "");

				TempData["PaymentSuccess"] = "Thanh toán thành công vui lòng kiểm tra đơn hàng ở Email";
				return RedirectToAction("Index", "Cart", TempData["PaymentSuccess"]);
			}

			return BadRequest(new { Message = "Không có sản phẩm trong giỏ hàng để đặt hàng." });
			//}
		}

		[HttpPost]
		public IActionResult createCheckoutSession(Customer customer , long? amount)
		{
			var currency = "VND";
			var host = HttpContext.Request.Host.Host; // Địa chỉ (host)
			var port = HttpContext.Request.Host.Port; // Cổng
			var scheme = HttpContext.Request.Scheme;
			StripeConfiguration.ApiKey = _stripeSettings.SecretKey;
			var options = new SessionCreateOptions
			{
				PaymentMethodTypes = new List<string>
				{
					"card"
				},
				LineItems = new List<SessionLineItemOptions>
				{
					new SessionLineItemOptions
					{
						PriceData = new SessionLineItemPriceDataOptions
						{
							Currency = currency,
							UnitAmount = amount,
							ProductData = new SessionLineItemPriceDataProductDataOptions
							{
								Name = "VTPharmacy",
								Description = customer.CustomerEmail + "-" + customer.CustomerName
							}

                        },
						Quantity =1
					}
				},
                Mode = "payment",
				SuccessUrl = Url.ActionLink("CreateOrder","Payment", customer),
				CancelUrl = Url.ActionLink("cancelStripe", "Payment")
            };
            var service = new SessionService();
            Session session = service.Create(options);
            return Redirect(session.Url);
        }
		
		public IActionResult cancelStripe()
		{
            TempData["PaymentCancel"] = "Thanh toán thất bại";
            return RedirectToAction("Index", "Cart", TempData["PaymentCancel"]);
        }
	}
}
