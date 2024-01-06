using System.ComponentModel.DataAnnotations;

namespace MultiShop.Areas.Manage.ViewModels
{
    public class CreateSizeVM
    {
        [Required(ErrorMessage = "The size name can't be empty")]
        [MaxLength(25, ErrorMessage = "The length of the size can't be more than 25")]
        public string Name { get; set; } = null!;
    }
}
