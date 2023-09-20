namespace Pharmacy.Pages
{
    public class PagingModel
    {
        public int currentPage { get; set; }
        public int countPage { get; set; }
        public Func<int?, string> generateUrl { get; set; }
    }
}
