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
                foreach (var item in ListSupplier)
                {
                    if (item.SupplierName.Contains(search))
                        SupplierFound.Add(item);
                }
                return SupplierFound;
            }

            return ListSupplier;
        }

        public void CreatSupplier(Supplier supplier)
        {
            _context.Add(supplier);
            _context.SaveChangesAsync();
        }

        public void DeleteSupplier(int id)
        {
            var item = _context.Suppliers.Find(id);
            _context.Suppliers.Remove(item);
            _context.SaveChangesAsync();
        }

        public Supplier GetSupplier(int id)
        {
            return _context.Suppliers.Find(id);
        }

        public async Task EditSupplierAsync(Supplier supplier)
        {
            var updateitem = _context.Suppliers.Find(supplier.SupplierId);
            updateitem.SupplierName = supplier.SupplierName;
            updateitem.SupplierEmail = supplier.SupplierEmail;
            updateitem.SupplierPhone = supplier.SupplierPhone;
            updateitem.SupplierAddress  = supplier.SupplierAddress;
            await _context.SaveChangesAsync();
        }
    }
}
