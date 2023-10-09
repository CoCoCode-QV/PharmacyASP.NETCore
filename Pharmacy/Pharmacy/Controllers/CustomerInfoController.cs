using Microsoft.AspNetCore.Mvc;
using Pharmacy.Data;
using Pharmacy.Models;
using System.Security.Claims;

namespace Pharmacy.Controllers
{
    public class CustomerInfoController : Controller
    {

        private readonly QlpharmacyContext _context;
        private readonly CustomerModels _customer;


        public CustomerInfoController(QlpharmacyContext context,CustomerModels customerModels)
        {
            _context = context;
            _customer = customerModels;
        }
        public IActionResult Index()
        {
            var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            
            if (userId != null)
            {
                ViewBag.UserId = true;
            }
            else
            {
                ViewBag.UserId = false;
            }

            var CustomerInfo = _customer.GetCustomer(userId);
            return View(CustomerInfo);
        }

        [HttpPost]
        public async Task<IActionResult> EditAsync(Customer item)
        {
        
            if (item.CustomerName == null || item.CustomerPhone == null || item.CustomerAddress == null || item.CustomerAge == null || item.CustomerAllergies == null || item.CustomerGender == null)
            {
                TempData["error"] = "Vui lòng điền đầy đủ thông tin";
                return RedirectToAction("Index");
            }
            else
            {
                TempData["success"] = "Cập nhật thông tin thành công!";
                await _customer.EditCustomer(item);
                return RedirectToAction("Index");
            }
        }

    }
}
