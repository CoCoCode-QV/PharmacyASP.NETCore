
using Pharmacy.Models;

namespace Pharmacy.ViewsModels
{
    public class ProductViewModels
    {
        public ProductCost ProductCost { get; set; }
        public IEnumerable<ProductCost> ListProductCost { get; set; }
        //public string DiscountName { get; set; }
        //public double? DiscountPercent { get; set; }
        public string SupplierName { get; set; }
        public string CategoryName { get; set; }
        public string AddressSupplier { get; set; }
        public Dictionary<int, double?> DiscountPercentMap { get; set; }
    }
}
