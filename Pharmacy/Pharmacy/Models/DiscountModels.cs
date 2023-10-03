using Pharmacy.Data;

namespace Pharmacy.Models
{
    public class DiscountModels
    {
        private readonly QlpharmacyContext _context;


        public DiscountModels(QlpharmacyContext context)
        {
            _context = context;
        }
        public IEnumerable<Discount> GetDiscount(string search)
        {
            var ListDiscount = _context.Discounts.OrderByDescending(category => category.DiscountId).ToList();
            if (search != null)
            {
                List<Discount> discountsFound = new List<Discount>();
                foreach (var item in ListDiscount)
                {
                    if (item.DiscountName.Contains(search))
                        discountsFound.Add(item);
                }
                return discountsFound;
            }

            return ListDiscount;
        }

        public async Task CreatDiscountAsync(Discount discount)
        {
            _context.Add(discount);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteDiscountAsync(int id)
        {
            var item = _context.Discounts.Find(id);
            _context.Discounts.Remove(item);
           await _context.SaveChangesAsync();
        }

        public Discount GetDiscount(int id)
        {
            return _context.Discounts.Find(id);
        }

        public async Task EditDiscount(Discount discount)
        {
            var updateitem = _context.Discounts.Find(discount.DiscountId);
            updateitem.DiscountName = discount.DiscountName;
            updateitem.DiscountPercent = discount.DiscountPercent;
            updateitem.DiscountStartDate = discount.DiscountStartDate;
            updateitem.DiscountEndDate  = discount.DiscountEndDate;
            await _context.SaveChangesAsync();
        }

        public Dictionary<int, double?> GetDiscountPercentMap(IEnumerable<Product> products, List<Discount> discounts)
        {
            Dictionary<int, double?> discountPercentMap = new Dictionary<int, double?>();
            foreach (var product in products)
            {
                Discount productDiscount = discounts.FirstOrDefault(p => p.DiscountId == product.DiscountId);
                if (productDiscount != null && productDiscount.DiscountEndDate > DateTime.Now)
                {
                    discountPercentMap[product.ProductId] = productDiscount.DiscountPercent;
                }
                else
                {
                    discountPercentMap[product.ProductId] = 0; // Không có giảm giá
                }
            }
            return discountPercentMap;
        }

       
    }
}
