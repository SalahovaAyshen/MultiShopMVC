using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MultiShop.Areas.Manage.ViewModels;
using MultiShop.DAL;
using MultiShop.Models;
using MultiShop.Utilities.Extensions;

namespace MultiShop.Areas.Manage.Controllers
{
    [Area("Manage")]
    
    public class SliderController : Controller
    {
        private readonly IWebHostEnvironment _env;

        public AppDbContext _context { get; }

        public SliderController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }


        public async Task<IActionResult> Index()
        {
            ICollection<Slider> slider = await _context.Sliders.ToListAsync();
            return View(slider);
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(CreateSliderVM sliderVM)
        {
            if (!ModelState.IsValid) return View();
            if(sliderVM.Order<=0 || await _context.Sliders.FirstOrDefaultAsync(s=>s.Order==sliderVM.Order) is not null)
            {
                ModelState.AddModelError("Order", "The order is negative number or already exists");
                return View();
            }
            if (!sliderVM.Photo.ValidateType("image/"))
            {
                ModelState.AddModelError("Photo", "The photo type must be image");
                return View();
            }
            if (!sliderVM.Photo.ValidateSize(2 * 1024))
            {
                ModelState.AddModelError("Photo", "The photo size can't be more than 2mb");
                return View();
            }
            string filename = await sliderVM.Photo.CreateFileAsync(_env.WebRootPath, "assets", "img");
            Slider slider = new Slider
            {
                Title = sliderVM.Title,
                Offer = sliderVM.Offer,
                Order = sliderVM.Order,
                Button = sliderVM.Button,
                ImageUrl = filename
            };
            await _context.Sliders.AddAsync(slider);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
