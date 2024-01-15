using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MultiShop.Models;

namespace MultiShop.DAL
{
    public class AppDbContext:IdentityDbContext<AppUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options):base(options) { }
        public DbSet<Setting> Settings { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Size> Sizes { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductSize> ProductSizes { get; set; }
        public DbSet<ProductImage> ProductImages { get; set; }
        public DbSet<Slider> Sliders { get; set; }
        public DbSet<Shipping> Shippings { get; set; }
        public DbSet<AppUser> AppUsers { get; set; }





    }
}
