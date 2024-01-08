using MultiShop.Models;

namespace MultiShop.Areas.Manage.ViewModels
{
    public class UpdateProductVM
    {
        public string Name { get; set; } = null!;
        public decimal Price { get; set; }
        public int Order { get; set; }
        public string Description { get; set; } = null!;
        public string? FullDescription { get; set; }
        public string SKU { get; set; } = null!;
        public int? CategoryId { get; set; }
        public ICollection<Category>? Categories { get; set; }
        public List<int> SizeIds { get; set; }
        public ICollection<Size>? Sizes { get; set; }
        public IFormFile? MainPhoto { get; set; }
        public ICollection<IFormFile>? AdditionalPhotos { get; set; }
        public List<int>? ImageIds { get; set; }

        public ICollection<ProductImage>? ProductImages { get; set; }
    }
}
