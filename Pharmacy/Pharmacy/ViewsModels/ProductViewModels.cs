using Pharmacy.Data;

namespace Pharmacy.ViewsModels
{
    public class ProductViewModels
    {
        public Product Product { get; set; }
        public IEnumerable<Product> ListProduct { get; set; }
        public string DiscountName { get; set; }
        public double? DiscountPercent { get; set; }
        public string SupplierName { get; set; }
        public string CategoryName { get; set; }
        public string AddressSupplier { get; set; }
        public Dictionary<int, double?> DiscountPercentMap { get; set; }
    }
}
