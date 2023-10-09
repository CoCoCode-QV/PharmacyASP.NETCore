using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.IdentityModel.Tokens;
using Pharmacy.Data;
using Pharmacy.Models;
using Pharmacy.ViewsModels;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Security.Policy;
using X.PagedList;

namespace Pharmacy.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class ProductController : Controller
    {
        private readonly ProductModels _ProductModels;
        private readonly QlpharmacyContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;



        public ProductController(ProductModels productModels, QlpharmacyContext context , IWebHostEnvironment webHostEnvironment) 
        {
            _ProductModels = productModels;
            _context = context;
            _webHostEnvironment = webHostEnvironment;

        }

        public const int Items_Per_Page = 3;
        public IActionResult Index(string search, string condition, int? page)
        {
            var listProducts = _ProductModels.GetProducts(search, condition);

            var pageNumber = page ?? 1;

            var onePage = listProducts.ToPagedList(pageNumber, Items_Per_Page);

            return View(onePage);
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
            if (photo != null && product.ProductName != null && product.ProductPrice != null && product.ProductInventory != null && product.ProductExpiryDate != null
                && product.ProductDetail != null && product.ProductIngredients != null && product.CategoryId != null && product.SupplierId != null && product.DiscountId != null)
            {
                try
                {
                    DateTime ExpiryDate = (DateTime)product.ProductExpiryDate;
                    DateTime currentDate = DateTime.Now.Date;

                    if(ExpiryDate < currentDate)
                    {
                        TempData["error"] = "Hạn sử dụng không hợp lệ";
                        return RedirectToAction("Create");
                    }
                    
                    string[] allowedExtensions = { ".jpg", ".jpeg", ".png", ".gif" }; // Các phần mở rộng cho hình ảnh cho phép
                    var fileExtension = Path.GetExtension(photo.FileName).ToLower(); // Lấy phần mở rộng của tệp

                    if (allowedExtensions.Contains(fileExtension))
                    {
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
                            ProductImage = Path.Combine("\\images", uniqueFileName),
                            ProductInventory = product.ProductInventory,
                            CategoryId = product.CategoryId,
                            SupplierId = product.SupplierId,
                            DiscountId = product.DiscountId,
                            ProductExpiryDate = product.ProductExpiryDate,
                            ProductIngredients = product.ProductIngredients,
                            ProductPrescription = product.ProductPrescription,
                            ProductActive = product.ProductActive,

                        };
                        await _ProductModels.CreatProduct(item);

                        // Redirect to the Index action after successful product creation
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        // Người dùng chọn tệp không hợp lệ, trả về lỗi
                        TempData["error"] = "Vui lòng chọn một tập tin hình ảnh hợp lệ (jpg, jpeg, png hoặc gif).";
                        return RedirectToAction("Create");
                    }
                
                }
                catch (Exception ex)
                {
                    // Handle the exception, log it, and return an error view or message
                    TempData["error"] = "Đã sãy ra lỗi khi di chuyển tệp ";
                    return RedirectToAction("Create");
                }
            }
            else
            {
                // Handle the case where ProductImage is null or empty
                TempData["error"] = "Vui lòng điền đầy đủ thông tin.";
                return RedirectToAction("Create");
            }
        }

        [HttpPost]
        public async Task<IActionResult> UploadImage([FromForm] IFormFile file)
        {
            if (file != null && file.Length > 0)
            {
                try
                {
                    string[] allowedExtensions = { ".jpg", ".jpeg", ".png", ".gif" }; // Các phần mở rộng cho hình ảnh cho phép
                    var fileExtension = Path.GetExtension(file.FileName).ToLower(); // Lấy phần mở rộng của tệp

                    if (allowedExtensions.Contains(fileExtension))
                    {
                        // Tạo đường dẫn tới thư mục lưu trữ hình ảnh trên máy chủ
                        string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "Detail");

                        // Tạo tên file duy nhất để tránh trùng lặp
                        string uniqueFileName = Guid.NewGuid().ToString() + "_" + file.FileName;
                        string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                        // Lưu file vào thư mục trên máy chủ
                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await file.CopyToAsync(fileStream);
                        }

                        // Trả về đường dẫn tới file đã tải lên
                        var imageUrl = Path.Combine("\\Detail", uniqueFileName);
                        var url = new ViewImageUploadResult { Url = imageUrl };
                        return Json(url);
                    }
                }
                catch (Exception ex)
                {
                    // Xử lý lỗi tải lên ở đây
                    return BadRequest("Error uploading image: " + ex.Message);
                }
            }

            // Trả về lỗi nếu không có file hoặc file rỗng
            return BadRequest("Invalid file.");
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

        public IActionResult Detail(int id)
        {
            Product product = _ProductModels.GetProduct(id);
            Discount discount = _context.Discounts.Where(p => p.DiscountId == product.DiscountId).SingleOrDefault();
            Supplier supplier = _context.Suppliers.Where(p => p.SupplierId == product.SupplierId).SingleOrDefault();
            Category category = _context.Categories.Where(p => p.CategoryId == product.CategoryId).SingleOrDefault();

            ViewBag.discountName = discount?.DiscountName;
            ViewBag.supplierName = supplier?.SupplierName;
            ViewBag.categoryName = category?.CategoryName;
            return View(product);
        }

        public IActionResult Edit(int id)
        {
            var data = _context.Products.Where(p => p.ProductId == id).SingleOrDefault();
            if( data != null)
            {
                getAll();
                return View(data) ;
            }
            else
            {
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        public async Task<IActionResult> EditAsync(Product product, IFormFile photo)
        {
            var data = _context.Products.Where(p => p.ProductId == product.ProductId).SingleOrDefault();

            if ( product.ProductName != null && product.ProductPrice != null && product.ProductInventory != null && product.ProductExpiryDate != null
                && product.ProductDetail != null && product.ProductIngredients != null && product.CategoryId != null && product.SupplierId != null && product.DiscountId != null)
            {
                try
                {
                    DateTime ExpiryDate = (DateTime)product.ProductExpiryDate;
                    DateTime currentDate = DateTime.Now.Date;

                    if (ExpiryDate < currentDate)
                    {
                        TempData["error"] = "Hạn sử dụng không hợp lệ";
                        getAll();
                        return View();
                    }
                    if(photo != null)
                    {
                       
                        string[] allowedExtensions = { ".jpg", ".jpeg", ".png", ".gif" }; // Các phần mở rộng cho hình ảnh cho phép
                        var fileExtension = Path.GetExtension(photo.FileName).ToLower(); // Lấy phần mở rộng của tệp

                        if (allowedExtensions.Contains(fileExtension))
                        {
                           

                            string webRootPath = _webHostEnvironment.WebRootPath;
                            var pathCurrrent = data.ProductImage.Substring(1);
                            string currentImage = Path.Combine(webRootPath, pathCurrrent);

                            if (currentImage != null)
                            {
                                if (System.IO.File.Exists(currentImage))
                                {
                                    System.IO.File.Delete(currentImage);
                                }
                            }
                            var uploadsFolder = Path.Combine("wwwroot", "images"); // Relative path to the wwwroot/images folder
                            var uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(photo.FileName);
                            var filePath = Path.Combine(uploadsFolder, uniqueFileName);
                            using (FileStream stream = new FileStream(filePath, FileMode.Create))
                            {
                                await photo.CopyToAsync(stream);
                                stream.Close();
                            }
                            string url = Path.Combine("\\images", uniqueFileName);
                            await _ProductModels.EditProductAsync(product, url);
                            return RedirectToAction("Index");
                        }
                        else
                        {
                            // Người dùng chọn tệp không hợp lệ, trả về lỗi
                            TempData["error"] = "Vui lòng chọn một tập tin hình ảnh hợp lệ (jpg, jpeg, png hoặc gif).";
                            getAll();
                            return View();
                        }

                    }
                    else
                    {
                        await _ProductModels.EditProductAsync(product, null);
                        return RedirectToAction("Index");
                    }
                }catch (Exception ex)
                {
                   
                    TempData["error"] = "Đã sãy ra lỗi khi di chuyển tệp ";
                    getAll();
                    return View(data);
                }
            }
            else
            {
                TempData["error"] = "Vui lòng điền đầy đủ thông tin.";
                getAll();
                return View(data);
            }
        }

        public async Task<IActionResult> DeleteAsync(int id)
        {
            if(id == 0)
            {
                return NotFound();
            }
            else
            {
                var data = _context.Products.Where(_ => _.ProductId == id).SingleOrDefault();
                if( data != null)
                {
                   // Không cần thêm wwwroot ở đây
                    string webRootPath = _webHostEnvironment.WebRootPath;
                    var pathCurrrent = data.ProductImage.Substring(1);
                    string currentImage = Path.Combine(webRootPath, pathCurrrent); 
                    
                    if (currentImage != null )
                    {
                        if (System.IO.File.Exists(currentImage))
                        {
                            System.IO.File.Delete(currentImage);
                        }
                    }

                    // Lấy nội dung chi tiết sản phẩm
                    var productDetailHtml = data.ProductDetail;
                    var imagePaths = GetImagePathsFromHtml(productDetailHtml);
                    // Xóa các hình ảnh từ thư mục
                    foreach (var imagePath in imagePaths)
                    {
                        var item = imagePath.Substring(1);
                        string fullImagePath = Path.GetFullPath(Path.Combine(webRootPath, item));
                        if (System.IO.File.Exists(fullImagePath))
                        {
                            System.IO.File.Delete(fullImagePath);
                        }
                    }
                    _context.Products.Remove(data);
                    await _context.SaveChangesAsync();

                }
            }
            return RedirectToAction("Index");
        }

        private List<string> GetImagePathsFromHtml(string html)
        {
            // Phân tích chuỗi HTML để tìm các đường dẫn hình ảnh
            var imagePaths = new List<string>();
            var doc = new HtmlAgilityPack.HtmlDocument();
            doc.LoadHtml(html);

            var imgNodes = doc.DocumentNode.SelectNodes("//img");
            if (imgNodes != null)
            {
                foreach (var imgNode in imgNodes)
                {
                    var srcAttribute = imgNode.Attributes["src"];
                    if (srcAttribute != null)
                    {
                        imagePaths.Add(srcAttribute.Value);
                    }
                }
            }

            return imagePaths;
        }
    }
}
