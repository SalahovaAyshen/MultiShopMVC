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
            ICollection<Size> size = await _context.Sizes.Skip((page-1)*3).Take(3).Include(c=>c.ProductSizes).ToListAsync(); 
            PaginationVM<Size> paginationVM = new PaginationVM<Size> 
            {
                TotalPage=Math.Ceiling((double)count/3),
                CurrentPage=page,
                Items = size
            };
            return View(paginationVM);
        }

    }
}
