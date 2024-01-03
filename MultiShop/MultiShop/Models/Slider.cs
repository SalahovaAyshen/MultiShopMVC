namespace MultiShop.Models
{
    public class Slider
    {
        public int Id { get; set; }
        public string ImageUrl { get; set; } = null!;
        public string Title { get; set; } = null!;
        public string? Offer { get; set; }
        public string Button { get; set; } = null!;
        public int Order { get; set; }

    }
}
