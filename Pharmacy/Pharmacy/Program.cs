using Amazon.Extensions.NETCore.Setup;
using Amazon.Runtime;
using Amazon.S3;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;
using Pharmacy.Hubs;
using Pharmacy.Models;
using Pharmacy.ViewsModels;
using Stripe;
using System.Configuration;
using System.Data.Common;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.


builder.Services.AddControllersWithViews().AddRazorRuntimeCompilation();
builder.Services.AddRazorPages();


builder.Services.AddScoped<Pharmacy.Models.CategoryModels>();
builder.Services.AddScoped<Pharmacy.Models.SupplierModels>();
builder.Services.AddScoped<Pharmacy.Models.UserModels>();
builder.Services.AddScoped<Pharmacy.Models.DiscountModels>();
builder.Services.AddScoped<Pharmacy.Models.ProductModels>();
builder.Services.AddScoped<Pharmacy.Models.CustomerModels>();
builder.Services.AddScoped<Pharmacy.Models.ProductCostModel>();
builder.Services.AddScoped<Pharmacy.Models.CartModels>();
builder.Services.AddScoped<Pharmacy.Models.OrderModels>();
builder.Services.AddScoped<Pharmacy.ViewsModels.StripeSettings>();


builder.Services.AddIdentity<IdentityUser, IdentityRole>().AddEntityFrameworkStores<QlpharmacyContext>().AddDefaultTokenProviders();

builder.Services.AddSignalR();


builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession();
builder.Services.AddHttpContextAccessor();
builder.Services.AddSession(options =>
{
	options.IdleTimeout = TimeSpan.FromMinutes(10);
});

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession();
builder.Services.AddHttpContextAccessor();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(10);
});
//Đăng nhập google
builder.Services.AddAuthentication(options =>
    {
        options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
    })
    .AddCookie()
    .AddGoogle(googleOptions =>
    {
      
        // Đọc thông tin Authentication:Google từ appsettings.json
        IConfigurationSection googleAuthNSection = builder.Configuration.GetSection("Authentication:Google");
        // Thiết lập ClientID và ClientSecret để truy cập API google
        googleOptions.ClientId = googleAuthNSection["ClientId"];
        googleOptions.ClientSecret = googleAuthNSection["ClientSecret"];
        // Cấu hình Url callback lại từ Google (không thiết lập thì mặc định là /signin-google)
        googleOptions.CallbackPath = "/LoginGoogle";

    });

Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
// Truy cập IdentityOptions
builder.Services.Configure<IdentityOptions>(options => {
    // Thiết lập về Password
    options.Password.RequireDigit = false; // Không bắt phải có số
    options.Password.RequireLowercase = false; // Không bắt phải có chữ thường
    options.Password.RequireNonAlphanumeric = false; // Không bắt ký tự đặc biệt
    options.Password.RequireUppercase = false; // Không bắt buộc chữ in
    options.Password.RequiredLength = 6; // Số ký tự tối thiểu của password
    options.Password.RequiredUniqueChars = 1; // Số ký tự riêng biệt

    // Cấu hình Lockout - khóa user
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(2); // Khóa 2 phút
    options.Lockout.MaxFailedAccessAttempts = 5; // Thất bại 5 lần thì khóa
    options.Lockout.AllowedForNewUsers = true;

    // Cấu hình về User.
    options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+!#$%^&*()[]{}<>|=/\\;:'\"`~, áàảãạăắằẳẵặâấầẩẫậéèẻẽẹêếềểễệíìỉĩịóòỏõọôốồổỗộơớờởỡợúùủũụưứừửữựýỳỷỹỵ";
    options.User.RequireUniqueEmail = true;  // Email là duy nhất

    
    // Cấu hình đăng nhập.
    options.SignIn.RequireConfirmedEmail = true;            // Cấu hình xác thực địa chỉ email (email phải tồn tại)
    options.SignIn.RequireConfirmedPhoneNumber = false;     // Xác thực số điện thoại

    options.Tokens.EmailConfirmationTokenProvider = "Default";
});


IConfigurationSection stripeSettings = builder.Configuration.GetSection("StripeSettings");
builder.Services.Configure<StripeSettings>(stripeSettings);

// Đăng ký StripeSettings để có thể sử dụng Dependency Injection
builder.Services.AddSingleton(provider => provider.GetRequiredService<IOptions<StripeSettings>>().Value);


//config aws_s3
IConfigurationSection s3Config = builder.Configuration.GetSection("AWS_S3");
builder.Services.AddSingleton<IAmazonS3>(new AmazonS3Client(s3Config["AWS_BUCKET_ACCESS_KEY"], s3Config["AWS_BUCKET_SECRET_KEY"], Amazon.RegionEndpoint.GetBySystemName("ap-southeast-1")));

//

builder.Services.AddTransient<ViewMailSettingcs>();

IConfigurationSection Database = builder.Configuration.GetSection("ConnectionStrings");
builder.Services.AddDbContext<QlpharmacyContext>(options =>
    //options.UseSqlServer("Server=KIMTAI;Database=QLPharmacy;Trusted_Connection=True;TrustServerCertificate=True;"));
    options.UseSqlServer(Database["DefaultConnection"]));

var app = builder.Build();
// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseStaticFiles();

app.UseSession();

app.UseHttpsRedirection();
app.UseSession();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization(); // xác thực quyền truy cập

app.MapHub<ChatHub>("/ChatHub");

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

using(var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var roles = new[] { "Admin", "Staff", "Member", "SuperAdmin" };

    foreach(var role in roles)
    {
        if(!await roleManager.RoleExistsAsync(role))
            await roleManager.CreateAsync(new IdentityRole(role));
    }
}
using (var scope = app.Services.CreateScope())
{
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();

    string email = "vanho12022002@gmail.com";
    string password = "Admin12345";

    if (await userManager.FindByEmailAsync(email) == null)
    {
        var user = new IdentityUser();
        user.UserName = email;
        user.Email = email;
        user.EmailConfirmed = true;

        await userManager.CreateAsync(user, password);
        await userManager.AddToRoleAsync(user, "SuperAdmin");

    }
}
//using (var scope = app.Services.CreateScope())
//{
//    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();

//    string email = "staff@gmail.com";
//    string password = "Staff123";

//    if (await userManager.FindByEmailAsync(email) == null)
//    {
//        var user = new IdentityUser();
//        user.UserName = email;
//        user.Email = email;
//        user.EmailConfirmed = true;

//        await userManager.CreateAsync(user, password);
//        await userManager.AddToRoleAsync(user, "Staff");

//    }
//}



app.Run();
