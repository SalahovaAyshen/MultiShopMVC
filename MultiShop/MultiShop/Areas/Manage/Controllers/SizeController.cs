using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MultiShop.Areas.Manage.ViewModels;
using MultiShop.DAL;
using MultiShop.Models;

namespace MultiShop.Areas.Manage.Controllers
{
    [Area("Manage")]
    public class SizeController : Controller
    {
        private readonly AppDbContext _context;

        public SizeController(AppDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index(int page = 1)
        {
            int count = await _context.Sizes.CountAsync();
            ICollection<Size> size = await _context.Sizes.Skip((page - 1) * 3).Take(3).Include(c => c.ProductSizes).ToListAsync();
            PaginationVM<Size> paginationVM = new PaginationVM<Size>
            {
                TotalPage = Math.Ceiling((double)count / 3),
                CurrentPage = page,
                Items = size
            };
            return View(paginationVM);
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(CreateSizeVM sizeVM)
        {
            if (!ModelState.IsValid) return View();
            bool result = await _context.Sizes.AnyAsync(s => s.Name == sizeVM.Name);
            if (result)
            {
                ModelState.AddModelError("Name", "The name already exists");
                return View();
            }
            Size size = new Size
            {
                Name = sizeVM.Name,
            };
            await _context.Sizes.AddAsync(size);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Update(int id)
        {
            if (id <= 0) return BadRequest();
            Size size = await _context.Sizes.FirstOrDefaultAsync(s => s.Id == id);
            if (size == null) return NotFound();
            UpdateSizeVM sizeVM = new UpdateSizeVM { Name = size.Name };
            return View(sizeVM);
        }
        [HttpPost]
        public async Task<IActionResult> Update(int id, UpdateSizeVM sizeVM)
        {
            if(id<=0) return BadRequest();
            Size exist = await _context.Sizes.FirstOrDefaultAsync(s=>s.Id==id);
            if(exist is null) return NotFound();
            bool result = await _context.Sizes.AnyAsync(s=>s.Name==sizeVM.Name);
            if (result)
            {
                ModelState.AddModelError("Name", "The name already exists");
                return View(sizeVM);
            }
            exist.Name = sizeVM.Name;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0) return BadRequest();
            Size size = await _context.Sizes.FirstOrDefaultAsync(s => s.Id == id);
            if(size == null) return NotFound();
            _context.Sizes.Remove(size);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
