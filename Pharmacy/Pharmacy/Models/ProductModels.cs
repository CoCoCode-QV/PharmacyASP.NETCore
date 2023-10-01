using Pharmacy.Data;

namespace Pharmacy.Models
{
    public class ProductModels
    {
        private readonly QlpharmacyContext _context;


        public ProductModels(QlpharmacyContext context)
        {
            _context = context;
        }


        public IEnumerable<Product> GetProducts(string search, string condition)
        {
            var ListProducts = _context.Products.OrderByDescending(s => s.ProductId).ToList();

            if (search != null)
            {
                List<Product> ProductFound = new List<Product>();
                switch (condition)
                {
                    case "name":
                        foreach (var item in ListProducts)
                            if (item.ProductName.Contains(search))
                                ProductFound.Add(item);
                        break;
                }
                return ProductFound;
            }

            return ListProducts;
        }

        public async Task CreatProduct(Product product)
        {
            _context.Add(product);
           await _context.SaveChangesAsync();
        }

        public async void DeleteProduct(int id)
        {
            var item = _context.Products.Find(id);
            _context.Products.Remove(item);
           await _context.SaveChangesAsync();
        }

        public Product GetProduct(int id)
        {
            return _context.Products.Find(id);
        }

        public async Task EditProductAsync(Product product)
        {
            var updateitem = _context.Products.Find(product.SupplierId);
            updateitem.ProductName  = product.ProductName;
            updateitem.ProductPrice = product.ProductPrice;
            updateitem.ProductDetail = product.ProductDetail;
            updateitem.ProductImage = product.ProductImage;
            updateitem.ProductInventory = product.ProductInventory;
            updateitem.ProductExpiryDate    = product.ProductExpiryDate;
            updateitem.ProductPrescription = product.ProductPrescription;
            updateitem.ProductActive = product.ProductActive;
            await _context.SaveChangesAsync();
        }
    }
}
