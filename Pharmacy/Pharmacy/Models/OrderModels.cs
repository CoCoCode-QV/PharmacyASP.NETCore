using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Pharmacy.Data;
using Pharmacy.ViewsModels;
using System.Text;

namespace Pharmacy.Models
{
	public class OrderModels
	{
		private readonly QlpharmacyContext _context;


		public OrderModels(QlpharmacyContext context)
		{
			_context = context;
		}

		public async Task CreateOrder(Order item)
		{
			_context.Orders.Add(item);
			await _context.SaveChangesAsync();
		}

		public async Task CreateOrderDetail(OrderDetail item)
		{
			_context.OrderDetails.Add(item);
			await _context.SaveChangesAsync();
		}

		public IEnumerable<Order> GetListOrder(string tab)
		{
            var allOrders = _context.Orders.OrderByDescending(o => o.OrderId).ToList();
            List<Order> ordersToShow;
            if (tab == "accept")
            {
                ordersToShow = allOrders.Where(o => o.OrderAccept).ToList();
				return ordersToShow;
            }
            else
            {
                ordersToShow = allOrders.Where(o => !o.OrderAccept).ToList();
				return ordersToShow;
            }	

			return allOrders;
        }

		public IEnumerable<OrderDetail> GetListOrderDetailByOrderId(int id)
		{
			return  _context.OrderDetails.Include(od => od.Order).Include(od => od.Product).Where(x => x.OrderId == id).ToList();
		}

		public async Task AcceptOrder(int id)
		{
			var item =	_context.Orders.Find(id);

			if(item != null)
			{
				item.OrderAccept = true;
				await _context.SaveChangesAsync();
			}
        }

		public IEnumerable<Order> GetListOrderByCustomerId(int id, string tab)
		{
			var allOrders = _context.Orders.Where(o=>o.CustomerId == id).OrderByDescending(o => o.OrderId).ToList();
            List<Order> ordersToShow;
            if (tab == "accept")
            {
                ordersToShow = allOrders.Where(o => o.OrderAccept).ToList();
                return ordersToShow;
            }
            else
            {
                ordersToShow = allOrders.Where(o => !o.OrderAccept).ToList();
                return ordersToShow;
            }

            return allOrders;
        }

        public async Task DeleteOrderAsync(int id)
        {
            var order = _context.Orders.Include(o => o.OrderDetails).ThenInclude(od => od.Product).Include(o=>o.Customer).FirstOrDefault(o => o.OrderId == id);

            if (order != null)
            {
                var orderDetailsList = order.OrderDetails.ToList();

                SendMailService sendMailService = new SendMailService();
                string emailBody = $@"<html>
                <head></head>
                <body>
					    <p>Xin lỗi đơn hàng của bạn đã bị hủy:</p>
					    {GenerateOrderDetailTable(orderDetailsList)}
                        
                        <em>Chúng tôi sẽ hoàn tiền lại cho quý khách sau mail này, nếu quý khách chưa nhận được tiền vui lòng liên hệ lại cho chúng tôi</em>
                               
                </body>
                </html>";
                sendMailService.SendMail(order.Customer.CustomerEmail, "VTPharmacy hủy đơn hàng", emailBody, "");

                // Xóa các chi tiết đơn hàng 
                _context.OrderDetails.RemoveRange(order.OrderDetails);
                // Sau đó xóa đơn hàng chính
                _context.Orders.Remove(order);

                // Lưu thay đổi vào cơ sở dữ liệu
                await _context.SaveChangesAsync();
            }
        }

        public string GenerateOrderDetailTable(List<OrderDetail> orderDetails)
        {

            StringBuilder htmlTable = new StringBuilder();
            htmlTable.Append("<table style='width:100%;border-collapse: collapse;' border='1'><tr><th style='text-align:left;padding: 10px;'>Tên sản phẩm</th><th style='text-align:center;padding: 10px;'>Giá (VNĐ)</th><th style='text-align:center;padding: 10px;'>Số lượng</th><th style='text-align:center;padding: 10px;'>Tổng tiền(VNĐ)</th></tr>");

            foreach (var item in orderDetails)
            {
                htmlTable.Append("<tr>");
                htmlTable.Append("<td style='text-align:left;padding: 10px;'>" + item.Product.ProductName + "</td>");
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
