using Microsoft.AspNetCore.Mvc;
using Pharmacy.Data;
using Pharmacy.Models;

namespace Pharmacy.Controllers
{
    public class OrderController : Controller
    {
        private readonly QlpharmacyContext _context; 
        private readonly CartModels _cart;
        
        public OrderController(QlpharmacyContext context,CartModels cartModels)
        {
            _context = context;
            _cart = cartModels;
        }
        public IActionResult Index()
        {
            return View();
        }

        //[HttpPost]
        //public IActionResult CreateOrder(int CustomerId) {
        //    // Kiểm tra xem có cartdetail nào cho người dùng này không
        //    var cart = _cart.getCartByCustomerId(CustomerId);

        //    if (_context.CartDetails.Any(cd => cd.CartId == cart.CartId))
        //    {
        //        // Tạo một đơn đặt hàng mới
        //        var newOrder = new Order
        //        {
        //            OrderAddress = "Customer Address Placeholder", 
        //            OrderStatus = 0, 
        //            CustomerID = orderModel.CustomerID,
        //            OrderDate = DateTime.Now 
        //        };

        //        _context.Orders.Add(newOrder);
        //        _context.SaveChanges();

        //        var orderID = newOrder.OrderID;

        //        // Lấy cartdetails của người dùng
        //        var cartDetails = _context.CartDetails.Where(cd => cd.CartID == orderModel.CustomerID).ToList();

        //        // Di chuyển dữ liệu từ CartDetail sang OrderDetails
        //        foreach (var cartDetail in cartDetails)
        //        {
        //            var orderDetail = new OrderDetail
        //            {
        //                OrderID = orderID,
        //                ProductID = cartDetail.ProductID,
        //                OrderDetailsQuantity = cartDetail.CartDetailQuantity,
        //                OrderDetailsPrice = cartDetail.CartDetailTemporaryPrice
        //            };

        //            _context.OrderDetails.Add(orderDetail);
        //        }

        //        // Xóa cartdetails sau khi chuyển dữ liệu
        //        _context.CartDetails.RemoveRange(cartDetails);
        //        _context.SaveChanges();

        //        return Ok(new { Message = "Đơn đặt hàng đã được tạo thành công!" });
        //    }

        //    return BadRequest(new { Message = "Không có sản phẩm trong giỏ hàng để đặt hàng." });
    //    //}
    //}
    }
}
