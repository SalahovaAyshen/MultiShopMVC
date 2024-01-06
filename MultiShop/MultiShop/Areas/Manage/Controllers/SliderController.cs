using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MultiShop.DAL;
using MultiShop.Models;

namespace MultiShop.Areas.Manage.Controllers
{
    [Area("Manage")]
    
    public class SliderController : Controller
    {
        public AppDbContext _context { get; }

        public SliderController(AppDbContext context)
        {
            _context = context;
        }


        public async Task<IActionResult> Index()
        {
            ICollection<Slider> slider = await _context.Sliders.ToListAsync();
            return View(slider);
        }
    }
}
