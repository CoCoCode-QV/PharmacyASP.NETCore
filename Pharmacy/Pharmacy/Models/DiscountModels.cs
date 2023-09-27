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
    }
}
