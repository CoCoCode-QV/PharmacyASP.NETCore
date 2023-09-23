using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Pharmacy.ViewsModels;

namespace Pharmacy.Controllers
{
    public class LoginController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;

        public LoginController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager; 
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(SignInViewModels item)
        {
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(item.userName, item.Password, item.RememberMe, false);
                var user = await _userManager.FindByNameAsync(item.userName);
                if (user != null && !await _userManager.IsEmailConfirmedAsync(user))
                {
                    ModelState.AddModelError("Validation", "Vui lòng xác nhận email trước khi đăng nhập.");
                    return View();
                }
                if (result.Succeeded)
                {
                    if (user != null)
                    {
                        var isInRole = await _userManager.IsInRoleAsync(user, "Admin");
                        if (isInRole)
                        {
                     
                            return RedirectToAction("Index", "HomeAdmin", new { Areas = "Admin" });
                        }

                        return RedirectToAction("Index", "Home");
                    }
                }
                ModelState.AddModelError("Validation", "Tài khoản hoặc mật khẩu bạn không đúng!");

            }
            return View();
        }

 
        public async Task<IActionResult> SignOut()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index","Login");
        }


        [HttpGet]
        public IActionResult CreateUser()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser(CreateUserViewModel Item)
        {
            var errorMapping = new Dictionary<string, string>
            {
                {"DuplicateUserName", "Tên đăng nhập này đã được sử dụng. Vui lòng chọn tên đăng nhập khác."},
                {"DuplicateEmail", "Địa chỉ email này đã được đăng ký. Vui lòng sử dụng địa chỉ email khác."},
                {"PasswordTooShort", "Mật khẩu phải chứa ít nhất 6 ký tự."}
                
            };
            if (ModelState.IsValid)
            {
                IdentityUser user = new IdentityUser
                {
                    UserName = Item.UserName,
                    Email = Item.Email
                };
                var result = await _userManager.CreateAsync(user, Item.Password);
                if(result.Succeeded)
                {
                    ViewBag.RegisteredResult = "Yes";
                    var userSucceeded = await _userManager.FindByEmailAsync(user.Email);
                    string code = await _userManager.GenerateEmailConfirmationTokenAsync(userSucceeded);
                    var callbackUrl = Url.Action("ConfirmEmail", "Login", new { userEmail = Item.Email, code = code },Request.Scheme);
                    SendMailService sendMailService = new SendMailService();
                    string emailBody = $@"<html>
                    <head></head>
                    <body>
                        <p>Vui lòng xác nhận tài khoản bằng cách nhấn vào link <a href='{callbackUrl}'>Tại đây</a></p>
                    </body>
                    </html>";
                    sendMailService.SendMail(user.Email, "Xác nhận tài khoản", emailBody, "");
                    ModelState.Clear();
                }
                else
                {
                    ViewBag.RegisteredResult = "No";
                    foreach(var error in result.Errors)
                    {
                        string customError;
                        if (errorMapping.TryGetValue(error.Code, out customError))
                        {
                            ModelState.AddModelError("Validation", customError);
                        }
                        else
                        {
                            ModelState.AddModelError("Validation", "Có lỗi xảy ra khi đăng ký tài khoản.");
                        }

                    }
                }
            }
            return View();
        }

        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail(string userEmail, string code)
        {
            if(userEmail == null || code == null)
            {
                return View("Error");
            }
            var user = await _userManager.FindByEmailAsync(userEmail);
            var result = await _userManager.ConfirmEmailAsync(user, code);
            return View(result.Succeeded ? "ConfirmEmail" : "Error" );    
        }
    }
}
