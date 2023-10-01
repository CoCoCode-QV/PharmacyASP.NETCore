using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.IdentityModel.Tokens;
using Pharmacy.Data;
using Pharmacy.Models;
using Pharmacy.ViewsModels;
using System.Diagnostics.Metrics;
using X.PagedList;

namespace Pharmacy.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class ProductController : Controller
    {
        private readonly ProductModels _ProductModels;
        private readonly QlpharmacyContext _context;


        public ProductController(ProductModels productModels, QlpharmacyContext context ) 
        {
            _ProductModels = productModels;
            _context = context;
        }

        public const int Items_Per_Page = 10;
        public IActionResult Index(string search, string condition, int? page)
        {
            var listSupplier = _ProductModels.GetProducts(search, condition);

            var pageNumber = page ?? 1;

            var onePageSupplier = listSupplier.ToPagedList(pageNumber, Items_Per_Page);

            return View(onePageSupplier);
        }

        public IActionResult Create()
        {
            getAll();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Product product, IFormFile photo)
        {
          
            // Check if ProductImage is not null or empty
            if (photo != null)
            {
                try
                {
                    DateTime ExpiryDate = (DateTime)product.ProductExpiryDate;
                    DateTime currentDate = DateTime.Now.Date;

                    if(ExpiryDate < currentDate)
                    {
                        TempData["error"] = "Hạn sử dụng không hợp lệ";
                        getAll();
                        return View();
                    }
                    var uploadsFolder = Path.Combine("wwwroot", "images"); // Relative path to the wwwroot/images folder
                    var uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(photo.FileName);
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);
                    using (FileStream stream = new FileStream(filePath, FileMode.Create))
                    {
                        await photo.CopyToAsync(stream);
                        stream.Close();
                    }
                    var item = new Product
                    {
                        ProductName = product.ProductName,
                        ProductPrice = product.ProductPrice,
                        ProductDetail = product.ProductDetail,
                        ProductImage = Path.Combine("images", uniqueFileName),
                        ProductInventory = product.ProductInventory,
                        CategoryId = product.CategoryId,
                        SupplierId = product.SupplierId,
                        DiscountId = product.DiscountId,
                        ProductExpiryDate = product.ProductExpiryDate,
                        ProductIngredients = product.ProductIngredients,
                        ProductPrescription = product.ProductPrescription,
                        ProductActive   = product.ProductActive,

                    };
                    await _ProductModels.CreatProduct(item);

                    // Redirect to the Index action after successful product creation
                    return RedirectToAction("Index");
                
                }
                catch (Exception ex)
                {
                    // Handle the exception, log it, and return an error view or message
                    TempData["error"] = "Đã sãy ra lỗi khi di chuyển tệp ";
                    getAll();
                    return View();
                }
            }
            else
            {
                // Handle the case where ProductImage is null or empty
                TempData["error"] = "Vui lòng chọn một tập tin hình ảnh hợp lệ.";
                getAll();
                return View();
            }


        }


        public void getAll()
        {
            List<Category> categories = _context.Categories.ToList();
            List<Supplier> suppliers = _context.Suppliers.ToList();
            List<Discount> discounts = _context.Discounts.ToList();
       

            ViewBag.Category = new SelectList(categories, "CategoryId", "CategoryName");
            ViewBag.Supplier = new SelectList(suppliers, "SupplierId", "SupplierName");
            ViewBag.Discount = new SelectList(discounts, "DiscountId", "DiscountName");
         
        }
    }
}
