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

      
        public IEnumerable<Supplier> GetSuppliers(string search, string condition)
        {
            var ListSupplier = _context.Suppliers.OrderByDescending(s => s.SupplierId).ToList();
       
            if (search != null)
            {
                List<Supplier> SupplierFound = new List<Supplier>();
                switch (condition)
                {
                    case "name":
                        foreach (var item in ListSupplier)
                            if (item.SupplierName.Contains(search))
                                SupplierFound.Add(item); 
                        break;
                    case "email":
                        foreach (var item in ListSupplier)
                            if (item.SupplierEmail.Contains(search))
                                SupplierFound.Add(item);
                        break;
                    
                    case "phone":
                        foreach (var item in ListSupplier)
                            if (item.SupplierPhone.Contains(search))
                                SupplierFound.Add(item);
                        break;

                }
                return SupplierFound;
            }

            return ListSupplier;
        }

        public async Task CreatSupplier(Supplier supplier)
        {
            _context.Add(supplier);
           await _context.SaveChangesAsync();
        }

        public async Task DeleteSupplier(int id)
        {
            var item = _context.Suppliers.Find(id);
            _context.Suppliers.Remove(item);
           await _context.SaveChangesAsync();
        }

        public Supplier GetSupplier(int id)
        {
            return  _context.Suppliers.Find(id);
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
