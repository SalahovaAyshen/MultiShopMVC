namespace MultiShop.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public decimal Price { get; set; }
        public int Order { get; set; }
        public string Description { get; set; } = null!;
        public string? FullDescription { get; set; }
        public string SKU { get; set; } = null!;
        public int? CategoryId { get; set; }
        public Category Category { get; set; }
        public ICollection<ProductSize> ProductSizes { get; set; }
        public ICollection<ProductImage> ProductImages { get; set; }

    }
}
