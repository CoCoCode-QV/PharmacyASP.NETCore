using Microsoft.EntityFrameworkCore;
using Pharmacy.Data;

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

		public IEnumerable<Order> GetListOrder()
		{
			return _context.Orders.OrderByDescending(o => o.OrderId).ToList();
        }

		public IEnumerable<OrderDetail> GetListOrderDetailByOrderId(int id)
		{
			return  _context.OrderDetails.Include(x =>x.Product).Where(x => x.OrderId == id).ToList();
		}

	
	}
}
