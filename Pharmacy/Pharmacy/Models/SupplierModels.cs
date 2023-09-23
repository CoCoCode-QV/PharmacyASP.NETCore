using Pharmacy.Data;

namespace Pharmacy.Models
{
    public class SupplierModels
    {
        private readonly QlpharmacyContext _context;


        public SupplierModels(QlpharmacyContext context)
        {
            _context = context;
        }

        public IEnumerable<Supplier> GetSuppliers(string search)
        {
            var ListSupplier = _context.Suppliers.OrderByDescending(s => s.SupplierId).ToList();
            if (search != null)
            {
                List<Supplier> SupplierFound = new List<Supplier>();
                foreach (var item in SupplierFound)
                {
                    if (item.SupplierName.Contains(search))
                        SupplierFound.Add(item);
                }
                return SupplierFound;
            }

            return ListSupplier;
        }
    }
}
