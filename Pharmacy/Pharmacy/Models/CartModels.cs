
using Pharmacy.ViewsModels;
using System.Security.Claims;
using System.Text;

namespace Pharmacy.Models
{
    public class CartModels
    {
        private readonly QlpharmacyContext _context;


        public CartModels(QlpharmacyContext context)
        {
            _context = context;
        }
        public Cart getCartByCustomerId(int customerID)
        {
            return _context.Carts.FirstOrDefault(p => p.CustomerId == customerID);
        }
        public IEnumerable<CartDetail> GetCartDetailsByCartId(int CartID)
        {
            return _context.CartDetails.Where(p => p.CartId == CartID).ToList();

        }
        public async Task CreateCart(Cart item)
        {
            _context.Carts.Add(item);
            await _context.SaveChangesAsync();
        }
        public async Task CreateCartDetail(CartDetail item)
        {
            _context.CartDetails.Add(item);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateCartDetailAsync(CartDetail item)
        {
            var updateitem = _context.CartDetails.Find(item.CartDetailId);
            if (updateitem != null)
            {
                updateitem.CartDetailQuantity = item.CartDetailQuantity;
                updateitem.CartDetailTemporaryPrice = item.CartDetailTemporaryPrice;
                await _context.SaveChangesAsync();
            }
        }

        public async Task DeleteDetailAsync(int id)
        {
            var item = _context.CartDetails.Find(id);
            _context.CartDetails.Remove(item);
            await _context.SaveChangesAsync();
        }

        public int TotalQuantityCartDetail(int id)
        {
            var count = _context.CartDetails
                   .Where(cd => cd.CartId == id).Sum(cd => cd.CartDetailQuantity);
            return Convert.ToInt32(count);
        }
        public bool DoesCartHaveDetails(int cartId)
        {
            return _context.CartDetails.Any(cd => cd.CartId == cartId);
        }

		public string GenerateCartItemsTable(List<CartItemViewModels> cartItems)
		{
		
			StringBuilder htmlTable = new StringBuilder();
			htmlTable.Append("<table style='width:100%;border-collapse: collapse;' border='1'><tr><th style='text-align:left;padding: 10px;'>Tên sản phẩm</th><th style='text-align:center;padding: 10px;'>Giá (VNĐ)</th><th style='text-align:center;padding: 10px;'>Số lượng</th><th style='text-align:center;padding: 10px;'>Tổng tiền(VNĐ)</th></tr>");

			foreach (var item in cartItems)
			{
				htmlTable.Append("<tr>");
				htmlTable.Append("<td style='text-align:left;padding: 10px;'>" + item.ProductName + "</td>");
				htmlTable.Append("<td style='text-align:center;padding: 10px;'>" + string.Format("{0:N0} VNĐ", item.CartDetailPriceCurrent) + "</td>");
				htmlTable.Append("<td style='text-align:center;padding: 10px;'>" + item.CartDetailQuantity + "</td>");
				htmlTable.Append("<td style='text-align:center;padding: 10px;'>" + string.Format("{0:N0} VNĐ", item.CartDetailTemporaryPrice) + "</td>");
				htmlTable.Append("</tr>");
			}   
			// Tính tổng tiền
			var cartTotalPrice = cartItems.Sum(item => item.CartDetailTemporaryPrice);
			htmlTable.Append("<tr><td colspan='3' style='text-align:right;padding: 10px;'>Tổng cộng:</td><td style='text-align:center;padding: 10px;'>" + string.Format("{0:N0} VNĐ", cartTotalPrice) + "</td></tr>");

			htmlTable.Append("</table>");

			return htmlTable.ToString();
		}


	}
}
