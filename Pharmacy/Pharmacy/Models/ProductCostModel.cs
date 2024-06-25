using Amazon.Auth.AccessControlPolicy;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Pqc.Crypto.Falcon;


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
                        ProductCostFound = ListProductsCost.Where(item => item.Product.ProductName!.Contains(search)).ToList();
                        break;
                    case "suppliername":
                        ProductCostFound = ListProductsCost.Where(item => item.Supplier.SupplierName!.Contains(search)).ToList();
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
            return _context.ProductCosts.Find(id)!;
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

   
                updateitem!.CostPrice = item.CostPrice;
                updateitem.SupplierId = item.SupplierId;
                updateitem.ProductExpiryDate = item.ProductExpiryDate;
                updateitem.ProductInventory = item.ProductInventory;
                updateitem.ReceivingDate = item.ReceivingDate;
                if(item.ProductInventory <= 0)
                {

                updateitem.CostActive = false;
                }
                else
                {

                updateitem.CostActive = item.CostActive;
                }
                await _context.SaveChangesAsync();
            }
        }
        public bool CheckInventory(int Costid, int quantity)
        {
            var item = _context.ProductCosts.Find(Costid);
            if (item != null && item!.CostActive)
            {
                int newinventory = (int)(item.ProductInventory - quantity);

                if (newinventory < 0) return false;

                return true;
            }
            return false;
        }
        public async Task UpdateInventory(int Costid, int quantity)
        {
            var item = _context.ProductCosts.Find(Costid);
            if (item!.CostActive)
            {

                int newinventory = (int)(item.ProductInventory - quantity);
                if (newinventory <= 0)
                {
                    item.ProductInventory = 0;
                    item.CostActive = false;
                    await _context.SaveChangesAsync();

                }
                else
                {
                    item.ProductInventory = newinventory;
                    await _context.SaveChangesAsync();
                }
               
            }
        }
        public IEnumerable<ProductCost> productCostInventory(string content, string condition)
        {
            var ListProductsCost = _context.ProductCosts.Include(pc => pc.Product)
            .Include(pc => pc.Supplier).Where(pc => pc.ProductInventory >= 0). OrderByDescending(s => s.ProductInventory).ToList();

            if (content != null)
            {
                List<ProductCost> ProductCostFound = new List<ProductCost>();
                switch (condition)
                {
                    case "productname":
                        ProductCostFound = ListProductsCost.Where(item => item.Product.ProductName!.Contains(content)).ToList();
                        break;
                    case "suppliername":
                        ProductCostFound = ListProductsCost.Where(item => item.Supplier.SupplierName!.Contains(content)).ToList();
                        break;
                }

                return ProductCostFound;
            }
            return ListProductsCost;
        }

        public IEnumerable<ProductCost> productCostExpiry()
        {
            DateTime dateExpiry = DateTime.Now.Date.AddMonths(+1);
            var ListProductsCost = _context.ProductCosts.Include(pc => pc.Product)
            .Include(pc => pc.Supplier).Where(pc => pc.ProductInventory >= 0 && pc.ProductExpiryDate  <= dateExpiry).OrderByDescending(s => s.ProductInventory).ToList();

            return ListProductsCost;
        }
    }
}
