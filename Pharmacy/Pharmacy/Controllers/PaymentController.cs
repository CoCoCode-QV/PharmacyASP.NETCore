using Microsoft.AspNetCore.Mvc;
using Pharmacy.Data;
using Pharmacy.Models;
using Pharmacy.ViewsModels;

namespace Pharmacy.Controllers
{
    public class PaymentController : Controller
    {
		private readonly CartModels _cart;
		private readonly CustomerModels _customer;
		private readonly OrderModels _order;
		private readonly ProductModels _product;


		public PaymentController( CartModels cartModels, CustomerModels customer, OrderModels order, ProductModels product)
		{
			_cart = cartModels;
			_customer = customer;
			_order = order;
			_product = product;
		}
		[HttpPost]
        public IActionResult Index(int CustomerId)
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
			
			return View(customer);
		}

		[HttpPost]
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
					var orderDetail = new OrderDetail
					{
						OrderId = newOrder.OrderId,
						ProductId = cartDetail.ProductId,
						OrderDetailsQuantity = cartDetail.CartDetailQuantity,
						OrderDetailsPrice = cartDetail.CartDetailPriceCurrent,
						OrderDetailsTemporaryPrice = cartDetail.CartDetailTemporaryPrice
					};
					var itemproduct = _product.GetProduct(cartDetail.ProductId);
					var cartItem = new CartItemViewModels {
					
						ProductName = Convert.ToString(itemproduct.ProductName),
						ProductImage = itemproduct.ProductImage,
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
	}
}
