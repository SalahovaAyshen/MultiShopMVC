using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MultiShop.DAL;
using MultiShop.Models;
using MultiShop.ViewModels;

namespace MultiShop.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;

        public HomeController(AppDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            ICollection<Category> categories = await _context.Categories.Include(c=>c.Products).ToListAsync();
            ICollection<Slider> sliders = await _context.Sliders.OrderBy(s=>s.Order).ToListAsync();
            ICollection<Shipping> shippings = await _context.Shippings.ToListAsync();
            ICollection<Product> featured = await _context.Products.Include(f=>f.ProductImages.Where(pi=>pi.IsPrimary==true)).ToListAsync();
            ICollection<Product> recent = await _context.Products.OrderByDescending(r=>r.Order).Take(8).Include(r => r.ProductImages.Where(pi => pi.IsPrimary == true)).ToListAsync();
            HomeVM homeVM = new HomeVM 
            {
                Categories = categories,
                Sliders = sliders,
                Shippings = shippings,
                FeaturedProducts = featured,
                RecentProducts = recent,
            };
            return View(homeVM);
        }
    }
}
