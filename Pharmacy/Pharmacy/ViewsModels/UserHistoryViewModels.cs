namespace Pharmacy.ViewsModels
{
    public class UserHistoryViewModels
    {
        public int customerId { get; set; }
        public string UserName { get; set; }
        public string UserEmail { get; set; }
        public int QuantityOrder { get; set; }
        public double TotalPrice { get; set; }
    }
}
