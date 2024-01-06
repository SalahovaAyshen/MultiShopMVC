using MultiShop.Models;
using System.ComponentModel.DataAnnotations;

namespace MultiShop.Areas.Manage.ViewModels
{
    public class CreateCategoryVM
    {
        [Required(ErrorMessage ="The category name can't be empty")]
        [MaxLength(25, ErrorMessage ="The length of the category name can't be more than 25")]
        public string Name { get; set; } = null!;
    }
}
