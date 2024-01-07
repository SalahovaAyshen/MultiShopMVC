using System.ComponentModel.DataAnnotations;

namespace MultiShop.Areas.Manage.ViewModels
{
    public class UpdateSliderVM
    {
        [Required(ErrorMessage = "The title can't be empty")]
        [MaxLength(25, ErrorMessage = "The length of the slider title can't be more than 25 characters")]
        public string Title { get; set; } = null!;
        [Required(ErrorMessage = "The offer can't be empty")]
        [MaxLength(40, ErrorMessage = "The length of the slider offer can't be more than 40 characters")]
        public string Offer { get; set; }
        [Required(ErrorMessage = "The order can't be empty")]
        public int Order { get; set; }
        [Required(ErrorMessage = "The button can't be empty")]
        [MaxLength(25, ErrorMessage = "The length of the slider button can't be more than 25 characters")]
        public string Button { get; set; }
        public IFormFile? Photo { get; set; }
    }
}
