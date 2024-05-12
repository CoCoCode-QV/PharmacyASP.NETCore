
using Microsoft.EntityFrameworkCore;

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
            var ListDiscount = _context.Discounts.OrderByDescending(e => e.DiscountId).ToList();
            if (search != null)
            {
                List<Discount> discountsFound = _context.Discounts.Where(item => item.DiscountName.Contains(search)).ToList();
              
                return discountsFound;
            }

            return ListDiscount;
        }

        public IEnumerable<ProductDiscount> GetProductDiscount(string search)
        {
            var ListDiscountProduct = _context.ProductDiscounts
                            .Include(pd => pd.Discount) 
                            .Include(pd => pd.Cost).ThenInclude(c =>c.Supplier)
                             .Include(pd => pd.Cost) .ThenInclude(cost => cost.Product)
                            .OrderByDescending(e => e.ProductDiscountId)
                            .ToList();
            //if (search != null)
            //{
            //    List<ProductDiscount> discountsFound = _context.ProductDiscounts.Where(item => item.DiscountName.Contains(search)).ToList();

            //    return discountsFound;
            //}

            return ListDiscountProduct;
        }

        public async Task CreatDiscountAsync(Discount discount)
        {
            _context.Add(discount);
            await _context.SaveChangesAsync();
        }

        public async Task CreatProductDiscountAsync(ProductDiscount item)
        {
            _context.Add(item);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteDiscountAsync(int id)
        {
            var item = _context.Discounts.Find(id);
            _context.Discounts.Remove(item);
           await _context.SaveChangesAsync();
        }

        public async Task DeleteDiscountProductAsync(int id)
        {
            var item = _context.ProductDiscounts.Find(id);
            _context.ProductDiscounts.Remove(item);
            await _context.SaveChangesAsync();
        }

        public Discount GetDiscount(int id)
        {
            return _context.Discounts.Find(id)!;
        }

        public ProductDiscount GetDiscountProduct(int id)
        {
            return _context.ProductDiscounts.Where(p => p.CostId == id).SingleOrDefault()!;
        }

        public async Task EditDiscount(Discount discount)
        {
            var updateitem = _context.Discounts.Find(discount.DiscountId);
            updateitem.DiscountName = discount.DiscountName;
            updateitem.DiscountPercent = discount.DiscountPercent;
            //updateitem.DiscountStartDate = discount.DiscountStartDate;
            //updateitem.DiscountEndDate  = discount.DiscountEndDate;
            await _context.SaveChangesAsync();
        }
        public async Task EditDiscount(ProductDiscount item)
        {
            var updateitem = _context.ProductDiscounts.Find(item.ProductDiscountId);
            updateitem.DiscountStartDate = item.DiscountStartDate;
            updateitem.DiscountId = item.DiscountId;
            updateitem.CostId = item.CostId;
            updateitem.DiscountStartDate = item.DiscountStartDate;
            updateitem.DiscountEndDate = item.DiscountEndDate;
            await _context.SaveChangesAsync();
        }

        public Dictionary<int, double?> GetDiscountPercentMap(IEnumerable<ProductCost> ProductCost, List<Discount> discounts)
        {
            Dictionary<int, double?> discountPercentMap = new Dictionary<int, double?>();
            foreach (var productCost in ProductCost)
            {
                double? discountPercent = 0;

                foreach (var productDiscount in productCost.ProductDiscounts)
                {
                    var discount = discounts.FirstOrDefault(d => d.DiscountId == productDiscount.DiscountId);
                    if (discount != null &&
                        productDiscount.DiscountStartDate <= DateTime.Now &&
                        (productDiscount.DiscountEndDate == null || productDiscount.DiscountEndDate > DateTime.Now))
                    {
                        discountPercent = discount.DiscountPercent;
                        break; // Tìm thấy chiết khấu hợp lệ, không cần kiểm tra tiếp
                    }
                }
                discountPercentMap[productCost.CostId] = discountPercent;
            }
            return discountPercentMap;
        }


    }
}
