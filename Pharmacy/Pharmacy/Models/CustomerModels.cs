using Microsoft.EntityFrameworkCore;
using Pharmacy.Data;

namespace Pharmacy.Models
{
    public class CustomerModels
    {
        private readonly QlpharmacyContext _context;


        public CustomerModels(QlpharmacyContext context)
        {
            _context = context;
        }
        public async Task CreatCustomer(Customer item)
        {
            _context.Customers.Add(item);
            await _context.SaveChangesAsync();
        }

        public Customer GetCustomer(string id)
        {
            return _context.Customers.FirstOrDefault(p => p.UserID == id);
        }

        public async Task EditCustomer(Customer item)
        {
            var updateitem = _context.Customers.Find(item.CustomerId);
            if (updateitem != null)
            {
                updateitem.CustomerName = item.CustomerName;
                updateitem.CustomerPhone = item.CustomerPhone;
                updateitem.CustomerAddress = item.CustomerAddress;
                updateitem.CustomerGender = item.CustomerGender;
                updateitem.CustomerAge = item.CustomerAge;
                updateitem.CustomerAllergies = item.CustomerAllergies;
                await _context.SaveChangesAsync();
            }
        }

        public Customer GetCustomerByid(int id)
        {
            return _context.Customers.FirstOrDefault(c => c.CustomerId == id);
        }
    }
}