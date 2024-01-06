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
    }
}
