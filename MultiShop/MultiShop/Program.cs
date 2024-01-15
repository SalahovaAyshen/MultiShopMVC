using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MultiShop.DAL;
using MultiShop.Models;
using MultiShop.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<AppDbContext>(opt =>
{
    opt.UseSqlServer(builder.Configuration["ConnectionStrings:Default"]);
});
builder.Services.AddIdentity<AppUser, IdentityRole>(options =>
{

    options.Password.RequiredLength = 8;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireDigit = true;

    options.User.RequireUniqueEmail = true;
    options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789_";
    options.Lockout.MaxFailedAccessAttempts = 3;
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(7);
}).AddEntityFrameworkStores<AppDbContext>().AddDefaultTokenProviders();
builder.Services.AddScoped<LayoutService>();
var app = builder.Build();

app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
        name: "area",
        pattern: "{area:exists}/{controller=Dashboard}/{action=Index}/{id?}"
        );
});
app.MapControllerRoute(
    "default",
    "{controller=home}/{action=index}/{id?}"
    );

app.Run();
