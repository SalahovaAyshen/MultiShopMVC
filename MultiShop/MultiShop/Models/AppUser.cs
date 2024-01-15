using Microsoft.AspNetCore.Identity;

namespace MultiShop.Models
{
    public class AppUser:IdentityUser
    {
        public string Name { get; set; }
        public string Surname { get; set; }
    }
}
