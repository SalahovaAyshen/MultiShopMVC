using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MultiShop.Areas.Manage.ViewModels;
using MultiShop.DAL;
using MultiShop.Models;
using MultiShop.Utilities.Enums;
using MultiShop.Utilities.Extensions;

namespace MultiShop.Areas.Manage.Controllers
{
    [Area("Manage")]
    [Authorize(Roles =nameof(UserRole.Admin))]
    public class ProductController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public ProductController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }
        public async Task<IActionResult> Index(int page =1 )
        {
            int count = await _context.Products.CountAsync();
            ICollection<Product> products = await _context.Products
                .Skip((page-1)*3).Take(3)
                .Include(p => p.ProductImages.Where(pi => pi.IsPrimary == true)).ToListAsync();
            PaginationVM<Product> paginationVM = new PaginationVM<Product>
            {
                TotalPage = Math.Ceiling((double)count / 3),
                CurrentPage = page,
                Items = products
            };
            return View(paginationVM);
        }
        public async Task<IActionResult> Create()
        {
            CreateProductVM productVM = new CreateProductVM();
            productVM.Categories = await _context.Categories.ToListAsync();
            productVM.Sizes = await _context.Sizes.ToListAsync();
            return View(productVM);
        }
        [HttpPost]
        public async Task<IActionResult> Create(CreateProductVM productVM)
        {
            productVM.Categories = await _context.Categories.ToListAsync();
            productVM.Sizes = await _context.Sizes.ToListAsync();
            if(!ModelState.IsValid) return View(productVM);
            if(!await _context.Categories.AnyAsync(c => c.Id == productVM.CategoryId))
            {
                ModelState.AddModelError("CategoryId", "Not found category id");
                return View(productVM);
            }
            foreach (var item in productVM.SizeIds)
            {
                if(!await _context.Sizes.AnyAsync(s => s.Id == item))
                {
                    ModelState.AddModelError("SizeIds", "Not found size id");
                    return View(productVM);
                }
            }
            if (!productVM.MainPhoto.ValidateType("image/"))
            {
                ModelState.AddModelError("MainPhoto", "The image type must be image/");
                return View(productVM);
            }
            if (!productVM.MainPhoto.ValidateSize(2*1024))
            {
                ModelState.AddModelError("MainPhoto", "The image size can't be more than 2mb");
                return View(productVM);
            }
            ProductImage main = new ProductImage
            {
                IsPrimary = true,
                Alt = productVM.Name,
                ImageUrl = await productVM.MainPhoto.CreateFileAsync(_env.WebRootPath, "assets", "img")
            };

            Product product = new Product
            {
                Name = productVM.Name,
                Price = productVM.Price,
                Order = productVM.Order,
                Description = productVM.Description,
                FullDescription = productVM.FullDescription,
                SKU = productVM.SKU,
                CategoryId = productVM.CategoryId,
                ProductSizes = new List<ProductSize>(),
                ProductImages = new List<ProductImage> { main }

            };

            foreach (var item in productVM.SizeIds)
            {
                ProductSize productSize = new ProductSize
                {
                    SizeId = item
                };
                product.ProductSizes.Add(productSize);
            }

            if(productVM.AdditionalPhotos is not null)
            {
                foreach (var item in productVM.AdditionalPhotos)
                {
                    if (!item.ValidateType("image/"))
                    {
                        ModelState.AddModelError("AdditionalPhotos", "The image type must be image/");
                        return View(productVM);
                    }
                    if (!item.ValidateSize(2*1024))
                    {
                        ModelState.AddModelError("AdditionalPhotos", "The image size can't be more than 2mb");
                        return View(productVM);
                    }
                    product.ProductImages.Add(new ProductImage
                    {
                        IsPrimary = null,
                        Alt = productVM.Name,
                        ImageUrl = await item.CreateFileAsync(_env.WebRootPath, "assets", "img")
                    });
                }
            }

            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Update(int id)
        {
            if (id <= 0) return BadRequest();
            Product product = await _context.Products
                .Include(p=>p.ProductImages)
                .Include(p=>p.ProductSizes)
                .FirstOrDefaultAsync(p => p.Id == id);
            if (product is null) return NotFound();

            UpdateProductVM productVM = new UpdateProductVM
            {
                Name = product.Name,
                Price = product.Price,
                Order = product.Order,
                Description = product.Description,
                FullDescription = product.FullDescription,
                SKU = product.SKU,
                CategoryId = product.CategoryId,
                Categories = await _context.Categories.ToListAsync(),
                SizeIds = product.ProductSizes.Select(p => p.SizeId).ToList(),
                Sizes = await _context.Sizes.ToListAsync(),
                ProductImages = product.ProductImages
            
            };
            return View(productVM);

        }
        [HttpPost]
        public async Task<IActionResult> Update(int id, UpdateProductVM productVM)
        {
            Product existed = await _context.Products
                .Include(p => p.ProductImages)
                .Include(p => p.ProductSizes)
                .FirstOrDefaultAsync(p => p.Id == id);

            productVM.Categories=await _context.Categories.ToListAsync();
            productVM.Sizes = await _context.Sizes.ToListAsync();
            productVM.ProductImages = existed.ProductImages;

            if (existed is null) return NotFound();

            if (!ModelState.IsValid) return View(productVM);

            if(await _context.Categories.FirstOrDefaultAsync(c => c.Id == productVM.CategoryId) == null)
            {
                ModelState.AddModelError("CategoryId", "Not found category id");
                return View(productVM);
            }
            if (productVM.MainPhoto != null)
            {
                if (!productVM.MainPhoto.ValidateType("image/"))
                {
                    ModelState.AddModelError("MainPhoto", "The entered photo type does not match the required one");
                    return View(productVM);
                }
                if (!productVM.MainPhoto.ValidateSize(500))
                {
                    ModelState.AddModelError("MainPhoto", "The size of the photo is larger than required");
                    return View(productVM);
                }
            }
            existed.ProductSizes.RemoveAll(ps => !productVM.SizeIds.Exists(x => x == ps.SizeId));
            var sizeList = productVM.SizeIds.Where(si => !existed.ProductSizes.Any(x => x.SizeId == si));
            foreach (var size in sizeList)
            {
                existed.ProductSizes.Add(new ProductSize { SizeId = size });
            }
            if (productVM.MainPhoto != null)
            {
                string main = await productVM.MainPhoto.CreateFileAsync(_env.WebRootPath, "assets", "img");
                ProductImage exImage = existed.ProductImages.FirstOrDefault(pi => pi.IsPrimary == true);
                exImage.ImageUrl.DeleteFile(_env.WebRootPath, "assets", "img");
                existed.ProductImages.Remove(exImage);
                existed.ProductImages.Add(new ProductImage
                {
                    IsPrimary = true,
                    ImageUrl = main,
                    Alt = productVM.Name
                });

            }
            if (productVM.ImageIds == null)
            {
                productVM.ImageIds = new List<int>();
            }
            List<ProductImage> removeable = existed.ProductImages.Where(pi => !productVM.ImageIds.Exists(i => i == pi.Id) && pi.IsPrimary == null).ToList();
            foreach (ProductImage remIm in removeable)
            {
                remIm.ImageUrl.DeleteFile(_env.WebRootPath, "assets", "img");
                existed.ProductImages.Remove(remIm);
            }
            if (productVM.AdditionalPhotos != null)
            {
                foreach (IFormFile photos in productVM.AdditionalPhotos)
                {
                    if (!photos.ValidateType("image/"))
                    {
                        ModelState.AddModelError("AdditionalPhotos", "The entered photo type does not match the required one");
                        return View(productVM);
                    }
                    if (!photos.ValidateSize(500))
                    {
                        ModelState.AddModelError("AdditionalPhotos", "The size of the photo is larger than required");
                        return View(productVM);
                    }

                    existed.ProductImages.Add(new ProductImage
                    {
                        IsPrimary = null,
                        ImageUrl = await photos.CreateFileAsync(_env.WebRootPath, "assets", "img"),
                        Alt = productVM.Name
                    });
                }
            }

            existed.Name = productVM.Name;
            existed.Price = productVM.Price;
            existed.Description = productVM.Description;
            existed.FullDescription = productVM.FullDescription;
            existed.SKU = productVM.SKU;
            existed.Order = productVM.Order;
            existed.CategoryId = productVM.CategoryId;

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0) return BadRequest();
            Product product = await _context.Products.Include(p => p.ProductImages).FirstOrDefaultAsync(x => x.Id == id);
            if (product == null) return NotFound();
            if (product.ProductImages != null)
            {
                foreach (ProductImage item in product.ProductImages)
                {
                    item.ImageUrl.DeleteFile(_env.WebRootPath, "assets","img");

                }
            }
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


    }
}
