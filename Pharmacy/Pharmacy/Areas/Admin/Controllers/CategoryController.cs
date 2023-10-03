using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Pharmacy.Data;
using Pharmacy.Models;
using System.Security.Policy;
using X.PagedList;

namespace Pharmacy.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class CategoryController : Controller
    {

      
        private readonly CategoryModels _categoryModels;

       

        public CategoryController( CategoryModels categoryModels)
        {
            _categoryModels = categoryModels;
           
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
               await _categoryModels.CreatCategory(category);
                return RedirectToAction("Index");
            }
        }

        public async Task<IActionResult> Delete(int id)
        {
            await _categoryModels.DeleteCategory(id);
            return  RedirectToAction("Index");
        }

        public IActionResult Edit(int id)
        {
            
            return View(_categoryModels.GetCategory(id));
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
