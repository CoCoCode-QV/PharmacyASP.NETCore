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

        #region Client
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
           if(updateitem != null)
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
        #endregion


        #region server
        public IEnumerable<Customer> getListCustomer(string search, string condition)
        {
            var listCustomer = _context.Customers.OrderByDescending(o => o.CustomerId).ToList();
            if (search != null)
            {
                List<Customer> CustomerFound = new List<Customer>();
                switch (condition)
                {
                    case "name":
                        CustomerFound = _context.Customers.Where(item => item.CustomerName.Contains(search)).ToList();
                        break;
                    case "email":
                        CustomerFound = _context.Customers.Where(item => item.CustomerEmail.Contains(search)).ToList();
                        break;

                    case "phone":
                        CustomerFound = _context.Customers.Where(item => item.CustomerPhone.Contains(search)).ToList();
                        break;

                }
                return CustomerFound;
            }
            return listCustomer;
        }

        public IEnumerable<Order> historyPurchase(int id)
        {
          
            return _context.Orders.Include(o => o.OrderDetails).ThenInclude(od => od.Product).Where(o => o.OrderStatus == 1 && o.CustomerId == id).ToList();
        }
        #endregion
    }
}
