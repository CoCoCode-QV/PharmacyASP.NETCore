using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Pharmacy.Data;
using System.Configuration;

var builder = WebApplication.CreateBuilder(args);


// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();
builder.Services.AddDbContext<QlpharmacyContext>(options =>
    options.UseSqlServer("Server=LAPTOP-Q21GJI2Q;Database=QLPharmacy;Trusted_Connection=True;TrustServerCertificate=True;"));

var app = builder.Build();
// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseStaticFiles();

app.UseHttpsRedirection();
 

app.UseRouting();

app.UseAuthorization(); // xác thực quyền truy cập
app.UseAuthentication();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
        name: "default",
        pattern: "/{controller=Home}/{action=Index}/{id?}");

    endpoints.MapAreaControllerRoute(
       name: "Admin",
       pattern: "/{controller}/{action=Index}/{id?}",
       areaName: "Admin"
    ) ;
  
    endpoints.MapRazorPages();

});

app.Run();
