using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Pharmacy.Data;
using Pharmacy.Pages;

namespace Pharmacy.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CategoryController : Controller
    {

        private readonly QlpharmacyContext _context;

        public CategoryController(QlpharmacyContext context)
        {
            _context = context;
        }

        public const int Items_Per_Page = 4;
        public IActionResult Index([FromQuery(Name = "p")] int currentPage, int pagesize )
        {
            var ListCategory = _context.Categories.ToList();

            var totalItem = ListCategory.Count;
            if(pagesize <= 0) pagesize = Items_Per_Page;
            int countPages = (int)Math.Ceiling((double)totalItem / pagesize);

            if (currentPage > countPages) currentPage = countPages;
            if(currentPage < 1) currentPage = 1;

            var paginModel = new PagingModel()
            {
                countPage = countPages,
                currentPage = currentPage,
                generateUrl = (pageNumber) => Url.Action("Index", new
                {
                    p = pageNumber,
                    pagesize = pagesize
                })
            };

            ViewBag.pagingModel = paginModel;  
            ViewBag.totalItem = totalItem;

            var categories = ListCategory.Skip((currentPage -1) *pagesize)
                                            .Take(pagesize)   
                                            .ToList();

            return View(categories);
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
                _context.Add(category);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
        }

        public async Task<IActionResult> Delete(int id)
        {
            var item = _context.Categories.Find(id);
            _context.Categories.Remove(item);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        public IActionResult Edit(int id)
        {
            Category item = _context.Categories.Find(id);
            return View(item);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Category category)
        {
            if (category.CategoryName == null)
            {
                TempData["error"] = "Vui lòng điền đầy đủ thông tin danh mục ";
                return View();
            }
            else
            {
                
                var updateitem = _context.Categories.Find(category.CategoryId);
                updateitem.CategoryName = category.CategoryName;
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
        }

    }
}
