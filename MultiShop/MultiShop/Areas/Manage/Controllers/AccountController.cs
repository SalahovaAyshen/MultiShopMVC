using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MultiShop.Areas.Manage.ViewModels;
using MultiShop.Models;
using MultiShop.Utilities.Enums;
using MultiShop.Utilities.Extensions;
using System.Text.RegularExpressions;

namespace MultiShop.Areas.Manage.Controllers
{
    [Area("Manage")]
    public class AccountController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register(RegisterVM registerVM)
        {
            if (!ModelState.IsValid) return View();
            if (!registerVM.Name.Check())
            {
                ModelState.AddModelError("Name", "Name can't contain a number");
                return View();
            }
            if (!registerVM.Surname.Check())
            {
                ModelState.AddModelError("Surname", "Surname can't contain a number");
                return View();
            }
            string email = registerVM.Email;
            Regex regex = new Regex(@"^[a-zA-Z0-9.!#$%&’*+/=?^_`{|}~-]+@[a-zA-Z0-9-]+(?:\.[a-zA-Z0-9-]+)*$");
            if (!regex.IsMatch(email))
            {
                ModelState.AddModelError("Email", "wrong format email");
                return View();
            }

            AppUser appUser = new AppUser
            {
                Name = registerVM.Name,
                Surname = registerVM.Surname,
                UserName = registerVM.Username,
                Email = email,
            };
            IdentityResult result  = await _userManager.CreateAsync(appUser, registerVM.Password);
            if (!result.Succeeded)
            {
                foreach (IdentityError item in result.Errors)
                {
                    ModelState.AddModelError(String.Empty, item.Description);
                    return View();
                }
            }

            await _signInManager.SignInAsync(appUser, false);
            return RedirectToAction("Index", "Dashboard");
        }

        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginVM loginVM, string? returnurl)
        {
            if (!ModelState.IsValid) return View();
            AppUser user = await _userManager.FindByNameAsync(loginVM.UsernameOrEmail);
            if(user is null)
            {
                user = await _userManager.FindByEmailAsync(loginVM.UsernameOrEmail);
                if (user is null)
                {
                    ModelState.AddModelError(String.Empty, "Username, email or password is incorrect");
                    return View();
                }
            }

           var result =await _signInManager.PasswordSignInAsync(user, loginVM.Password, loginVM.IsRemembered, true);
            if (result.IsLockedOut)
            {
                ModelState.AddModelError(String.Empty, "Locked");
                return View();
            }
            if (!result.Succeeded)
            {
                ModelState.AddModelError(String.Empty, "Username, email or password is incorrect");
                return View();
            }

            if (returnurl == null)
            {
                return RedirectToAction("Index", "Dashboard");
            }
            return Redirect(returnurl);
        }
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Dashboard");
        }

        public async Task<IActionResult> CreateRoles()
        {
            foreach (var item in Enum.GetValues(typeof(UserRole)))
            {
                if(!await _roleManager.RoleExistsAsync(item.ToString()))
                {
                    await _roleManager.CreateAsync(new IdentityRole
                    {
                        Name = item.ToString(),
                    });
                }
            }
            return RedirectToAction("Index", "Dashboard");
        }
    }
}
