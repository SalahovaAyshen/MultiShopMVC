using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MultiShop.Areas.Manage.ViewModels;
using MultiShop.DAL;
using MultiShop.Models;

namespace MultiShop.Areas.Manage.Controllers
{
    [Area("Manage")]

    public class CategoryController : Controller
    {
        private readonly AppDbContext _context;

        public CategoryController(AppDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index(int page = 1)
        {
            int count = await _context.Categories.CountAsync();

            ICollection<Category> categories = await _context.Categories.Skip((page - 1) * 3).Take(3).Include(c => c.Products).ToListAsync();

            PaginationVM<Category> pagination = new PaginationVM<Category>
            {
                TotalPage = Math.Ceiling((double)count / 3),
                CurrentPage = page,
                Items = categories
            };
            return View(pagination);
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(CreateCategoryVM categoryVM)
        {
            if (!ModelState.IsValid) return View();
            bool result = await _context.Categories.AnyAsync(c => c.Name.ToLower().Trim() == categoryVM.Name.ToLower().Trim());
            if (result)
            {
                ModelState.AddModelError("Name", "This category already exists");
                return View();
            }
            Category category = new Category
            {
                Name = categoryVM.Name
            };
            await _context.Categories.AddAsync(category);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        
        public async Task<IActionResult> Update(int id)
        {
            if (id <= 0) return BadRequest();
            Category category = await _context.Categories.FirstOrDefaultAsync(c => c.Id == id);
            if (category == null) return NotFound();
            UpdateCategoryVM categoryVM = new UpdateCategoryVM
            {
                Name = category.Name
            };
            return View(categoryVM);
        }
        [HttpPost]
        public async Task<IActionResult> Update(int id, UpdateCategoryVM categoryVM)
        {
            if(id<=0) return BadRequest();
            Category exist = await _context.Categories.FirstOrDefaultAsync(c=>c.Id== id);   
            if(exist==null) return NotFound();
            bool result = await _context.Categories.AnyAsync(c => c.Name == categoryVM.Name);
            if(result)
            {
                ModelState.AddModelError("Name", "The name already exists");
                return View(categoryVM);
            }
            exist.Name = categoryVM.Name;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0) return BadRequest();
            Category category = await _context.Categories.FirstOrDefaultAsync(c => c.Id == id);
            if (category == null) return NotFound();
            Product product = await _context.Products.FirstOrDefaultAsync(c => c.CategoryId == id);
            if(product is not null)
            {
                product.CategoryId = null;
            }
            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));

        }
    }
}
