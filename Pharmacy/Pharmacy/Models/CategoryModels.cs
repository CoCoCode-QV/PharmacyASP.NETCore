using Pharmacy.Data;
using System.ComponentModel;

namespace Pharmacy.Models
{
    public class CategoryModels
    {
        private readonly QlpharmacyContext _context;

   
        public CategoryModels(QlpharmacyContext context)
        {
            _context = context;
        }
        public IEnumerable<Category> GetCategory(string search)
        {
            var ListCategory = _context.Categories.OrderByDescending(category => category.CategoryId).ToList();
            if (search != null)
            {
                List<Category> categoriesFound = new List<Category>();
                foreach(var item in  ListCategory)
                {
                    if (item.CategoryName.Contains(search))
                        categoriesFound.Add(item);
                }                  
                return categoriesFound;
            }
          
            return ListCategory;
        }

        public  void CreatCategory(Category category)
        {
            _context.Add(category);
            _context.SaveChangesAsync();
        }

        public void DeleteCategory(int id) {
            var item = _context.Categories.Find(id);
            _context.Categories.Remove(item);
             _context.SaveChangesAsync();
        }

        public Category GetCategory(int id)
        {
            return _context.Categories.Find(id);
        }

        public  void EditCategory(Category category)
        {
            var updateitem = _context.Categories.Find(category.CategoryId);
            updateitem.CategoryName = category.CategoryName;
             _context.SaveChangesAsync();
        }
    }
}
