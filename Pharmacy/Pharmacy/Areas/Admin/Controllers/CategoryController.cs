using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

using Pharmacy.Models;
using Stripe;
using System.Security.Policy;
using X.PagedList;

namespace Pharmacy.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin, Staff, SuperAdmin")]
    public class CategoryController : Controller
    {

      
        private readonly CategoryModels _categoryModels;
        private readonly QlpharmacyContext _context;

        public CategoryController( CategoryModels categoryModels, QlpharmacyContext context)
        {
            _categoryModels = categoryModels;
            _context = context;
           
        }

        public const int Items_Per_Page = 3;
        public IActionResult Index( string search,int? page )
        {
          
            var ListCategory = _categoryModels.GetCategory(search);

            var pageNumber = page ?? 1;

            var OnePageCategories = ListCategory.ToPagedList(pageNumber, Items_Per_Page);

            ViewBag.itemIndex = (pageNumber - 1) * Items_Per_Page;
            return View(OnePageCategories);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Category category)
        {
            if (category.CategoryName == null)
            {
                TempData["error"] = "Vui lòng điền đầy đủ thông tin danh mục";
                return View();
            }
            else
            {
                List<Category> categories = _context.Categories.ToList();
                foreach (var item in categories)
                {
                    if(item.CategoryName.ToUpper() == category.CategoryName.ToUpper())
                    {
                        TempData["error"] = "Tên danh mục đã tồn tại";
                        return View();
                    }
                    
                }
                TempData["Success"] = "Thêm danh mục thành công";
                await _categoryModels.CreatCategory(category);
                return RedirectToAction("Index");
            }
        }

        public async Task<IActionResult> Delete(int id)
        {
            Category category = _categoryModels.GetCategoryid(id);
            if(category != null)
            {
   
                   
                    List<Product> products = _context.Products.ToList();
                    foreach (var item in products)
                    {
                        if (item.CategoryId == category.CategoryId)
                        {
                            TempData["Error"] = "Danh mục đã có trong sản phẩm không thể xóa";
                            return RedirectToAction("Index");
                        }
                    }
          
                    TempData["Success"] = "Danh mục đã được xóa thành công";
                    await _categoryModels.DeleteCategory(id);
                    return RedirectToAction("Index");
                        
            }
              TempData["Error"] = "Không tìm danh mục giá phù hợp";
       
            return  RedirectToAction("Index");
        }

        public IActionResult Edit(int id)
        {
            
            return View(_categoryModels.GetCategoryid(id));
        }

        [HttpPost]
        public IActionResult Edit(Category category)
        {
            if (category.CategoryName == null)
            {
                TempData["error"] = "Vui lòng điền đầy đủ thông tin danh mục ";
                return View();
            }
            else
            {
                _categoryModels.EditCategory(category);
                return RedirectToAction("Index");
            }
        }

    }
}
