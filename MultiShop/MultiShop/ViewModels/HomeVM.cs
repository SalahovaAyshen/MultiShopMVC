
using MultiShop.Models;

namespace MultiShop.ViewModels
{
    public class HomeVM
    {
        public ICollection<Category> Categories { get; set; }
        public ICollection<Slider> Sliders { get; set; }
        public ICollection<Shipping> Shippings { get; set; }
        public ICollection<Product> FeaturedProducts { get; set; }
        public ICollection<Product> RecentProducts { get; set; }


    }
}
