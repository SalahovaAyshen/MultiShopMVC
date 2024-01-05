using MultiShop.Models;

namespace MultiShop.ViewModels
{
    public class ShopVM
    {
        public ICollection<Product> Product { get; set; }
        public double TotalPage { get; set; }
        public int CurrentPage { get; set; }
    }
}
