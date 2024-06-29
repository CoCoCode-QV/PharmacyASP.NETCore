
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
                List<Category> categoriesFound =  _context.Categories.Where(item => item.CategoryName.Contains(search)).ToList();
                return categoriesFound;
            }
          
            return ListCategory;
        }

        public  async Task CreatCategory(Category category)
        {
            _context.Add(category);
           await _context.SaveChangesAsync();
        }

        public async Task DeleteCategory(int id) {
            var item = _context.Categories.Find(id);
            _context.Categories.Remove(item);
            await _context.SaveChangesAsync();
        }

        public Category GetCategoryid(int id)
        {
            return _context.Categories.Find(id);
        }

        public  async Task EditCategory(Category category)
        {
            var updateitem = _context.Categories.Find(category.CategoryId);
            updateitem.CategoryName = category.CategoryName;
            await _context.SaveChangesAsync();
        }

    }
}
