using Amazon.Auth.AccessControlPolicy;
using Microsoft.EntityFrameworkCore;


namespace Pharmacy.Models
{
    public class ProductCostModel
    {
        private readonly QlpharmacyContext _context;

        public ProductCostModel(QlpharmacyContext context)
        {
            _context = context;
        }

        public IEnumerable<ProductCost> GetProductsCost(string search, string condition)
        {
            var ListProductsCost = _context.ProductCosts.Include(pc => pc.Product)
            .Include(pc => pc.Supplier).OrderByDescending(s => s.CostId).ToList();

            if (search != null)
            {
                List<ProductCost> ProductCostFound = new List<ProductCost>();
                switch (condition)
                {
                    case "productname":
                        ProductCostFound = _context.ProductCosts.Where(item => item.Product.ProductName.Contains(search)).ToList();
                        break;
                    case "suppliername":
                        ProductCostFound = _context.ProductCosts.Where(item => item.Supplier.SupplierName.Contains(search)).ToList();
                        break;

                

                }
              
                return ProductCostFound;
            }
            return ListProductsCost;
        }

        public async Task CreateProductCost(ProductCost productcost)
        {
          
            var oldProductCosts = _context.ProductCosts.Where(pc => pc.ProductId == productcost.ProductId && pc.CostActive);

            foreach (var oldProductCost in oldProductCosts)
            {
                oldProductCost.CostActive = false;
            }

            _context.ProductCosts.Add(productcost);

            await _context.SaveChangesAsync();

        }

        public async Task DeleteProductCost(int id)
        {
            var item = _context.ProductCosts.Find(id);
            _context.ProductCosts.Remove(item);
            await _context.SaveChangesAsync();
        }

        public ProductCost GetProductCostById(int id)
        {
            return _context.ProductCosts.Find(id);
        }

        public async Task Edit(ProductCost item)
        {
            var updateitem = _context.ProductCosts.Find(item.CostId);
            if (item.CostActive)
            {
                var oldProductCosts = _context.ProductCosts.Where(pc => pc.ProductId == item.ProductId && pc.CostActive);

                if(oldProductCosts != null)
                {
                    foreach (var oldProductCost in oldProductCosts)
                    {
                        oldProductCost.CostActive = false;
                    }
                }

           
                updateitem.CostPrice = item.CostPrice;
                updateitem.SupplierId = item.SupplierId;
                updateitem.ReceivingDate = item.ReceivingDate;
                updateitem.CostActive = item.CostActive;
                await _context.SaveChangesAsync();
            }
        }
    }
}
