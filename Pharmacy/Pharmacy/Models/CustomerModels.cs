using Microsoft.EntityFrameworkCore;
using Pharmacy.ViewsModels;
using Stripe;


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
            return _context.Customers.FirstOrDefault(p => p.UserId == id);
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
                        CustomerFound = listCustomer.Where(item => item.CustomerName.Contains(search)).ToList();
                        break;
                    case "email":
                        CustomerFound = listCustomer.Where(item => item.CustomerEmail.Contains(search)).ToList();
                        break;

                    case "phone":
                        CustomerFound = listCustomer.Where(item => item.CustomerPhone.Contains(search)).ToList();
                        break;

                }
                return CustomerFound;
            }
            return listCustomer;
        }

        public IEnumerable<Order> historyPurchase(int id)
        {

            return _context.Orders.Include(o => o.OrderDetails).ThenInclude(od => od.Cost).ThenInclude(c=>c.Product).Where(o => o.OrderDelivery == 1 && o.CustomerId == id).ToList();
          
        }

        public IEnumerable<UserHistoryViewModels> FetchHistoryPurchaseCustomer(string search, string condition)
        {
          
            var userHistories = _context.Customers
             .Where(c => c.Orders.Any(o => o.OrderStatus == 1 && o.OrderDelivery == 1))
             .Select(c => new UserHistoryViewModels
             {
                 customerId = c.CustomerId,
                 UserName = c.CustomerName,
                 UserEmail = c.CustomerEmail,
                 QuantityOrder = c.Orders.Count(o => o.OrderStatus == 1 && o.OrderDelivery == 1),
                 TotalPrice = c.Orders
                     .Where(o => o.OrderStatus == 1 && o.OrderDelivery == 1)
                     .SelectMany(o => o.OrderDetails)
                     .Sum(od => od.OrderDetailsTemporaryPrice ?? 0)
             })
             .OrderByDescending(c => c.QuantityOrder)
             .ToList();

            if (search != null)
            {
                List<UserHistoryViewModels> CustomerFound = new List<UserHistoryViewModels>();
                switch (condition)
                {
                    case "name":
                        CustomerFound = userHistories.Where(item => item.UserName!.Contains(search)).ToList();
                        break;
                    case "email":
                        CustomerFound = userHistories.Where(item => item.UserEmail!.Contains(search)).ToList();
                        break;


                }
                return CustomerFound;
            }
            return userHistories;   
        }
        public IEnumerable<TopSaleCustomerViewModels> topSaleCustomer(int id)
        {
            var topProducts = _context.OrderDetails
              .Include(od => od.Order)
              .Include(od => od.Cost)
              .ThenInclude(c => c.Product)
              .Where(od => od.Order.CustomerId == id && od.Order.OrderStatus == 1 && od.Order.OrderDelivery == 1)
              .GroupBy(od => new { od.Cost.Product.ProductId, od.Cost.Product.ProductName, od.Cost.Product.ProductImage })
              .Select(g => new TopSaleCustomerViewModels
              {
                  ProductId = g.Key.ProductId,
                  ProductName = g.Key.ProductName!,
                  ProductImage = g.Key.ProductImage!,
                  TotalQuantityPurchased = g.Sum(od => od.OrderDetailsQuantity ?? 0),
                  TotalAmountSpent = g.Sum(od => od.OrderDetailsTemporaryPrice ?? 0)
              })
              .OrderByDescending(p => p.TotalQuantityPurchased)
              .Take(10)
              .ToList();

            return topProducts;
        }

        #endregion
    }
}
