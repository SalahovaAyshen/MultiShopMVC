using Microsoft.EntityFrameworkCore;
using MultiShop.DAL;
using MultiShop.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<AppDbContext>(opt =>
{
    opt.UseSqlServer(builder.Configuration["ConnectionStrings:Default"]);
});
builder.Services.AddScoped<LayoutService>();
var app = builder.Build();

app.UseStaticFiles();
app.UseRouting();

app.MapControllerRoute(
    name:"default",
    pattern:"{controller}/{action}");

app.Run();
