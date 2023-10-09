using Pharmacy.Data;
using System.Security.Claims;

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
            return  _context.Carts.FirstOrDefault(p => p.CustomerId == customerID);
        }
        public IEnumerable<CartDetail> GetCartDetailsByCartId(int CartID)
        {
           return  _context.CartDetails.Where(p=>p.CartId == CartID).ToList();
       
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
    }
}
