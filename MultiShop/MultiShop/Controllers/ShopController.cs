using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MultiShop.DAL;
using MultiShop.Models;
using MultiShop.ViewModels;

namespace MultiShop.Controllers
{
    public class ShopController : Controller
    {
        private readonly AppDbContext _context;

        public ShopController(AppDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index(int? order)
        {
            IQueryable<Product> query =  _context.Products.Include(p => p.ProductImages.Where(pi => pi.IsPrimary == true)).AsQueryable();
            switch (order)
            {
                case 1:
                    query = query.OrderBy(q => q.Name);
                    break;
                case 2:
                    query = query.OrderBy(q => q.Price);
                    break;
                case 3:
                    query = query.OrderByDescending(q => q.Price);
                    break;
                case 4:
                    query=query.OrderByDescending(q => q.Id);
                    break;
            }

            ShopVM vm = new ShopVM
            {
                Product = await query.ToListAsync()
            };
            return View(vm);
        }
    }
}
