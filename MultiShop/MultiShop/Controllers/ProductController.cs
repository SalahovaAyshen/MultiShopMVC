using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MultiShop.DAL;
using MultiShop.Models;
using MultiShop.ViewModels;

namespace MultiShop.Controllers
{
    public class ProductController : Controller
    {
        private readonly AppDbContext _context;

        public ProductController(AppDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Detail(int id)
        {
            if (id <= 0) return BadRequest();
            Product? product = await _context.Products
                .Include(p => p.ProductImages)
                .Include(p => p.Category)
                .Include(p => p.ProductSizes)
                .ThenInclude(ps => ps.Size)
                .FirstOrDefaultAsync(p=>p.Id==id);
            if(product==null) return NotFound();

            ICollection<Product> related = await _context.Products
                .Include(p => p.Category)
                .Where(p=>p.CategoryId==product.CategoryId && p.Id!=product.Id)
                .Include(p=>p.ProductImages.Where(pi=>pi.IsPrimary==true))
                .ToListAsync();

            ProductVM vm = new ProductVM
            {
                Product = product,
                RelatedProducts = related
            };
            return View(vm);
        }
    }
}
