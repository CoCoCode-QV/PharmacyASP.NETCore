
namespace Pharmacy.ViewsModels
{
    public class CartItemViewModels
    {
        public int CartDetailId { get; set; }
        public int CostId { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string ProductImage { get; set; }
        public double? CartDetailPriceCurrent { get; set; }
        public int? CartDetailQuantity { get; set; }
        public double? CartDetailTemporaryPrice { get; set; }
    }
}
