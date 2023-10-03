using Pharmacy.Data;
using X.PagedList;

namespace Pharmacy.ViewsModels
{
    public class ProductListViewModel
    {
        public IPagedList<Product> Products { get; set; }
        public Dictionary<int, double?> DiscountPercentMap { get; set; }
    }
}
