using MultiShop.Models;

namespace MultiShop.ViewModels
{
    public class ProductVM
    {
        public Product Product { get; set; }
        public ICollection<Product> RelatedProducts { get; set; }
    }
}
