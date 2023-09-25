using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using MimeKit;
using Pharmacy.ViewsModels;
using System.Security.Claims;
using System.Text;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication;
using Microsoft.VisualBasic;
using static Org.BouncyCastle.Crypto.Engines.SM2Engine;

namespace Pharmacy.Controllers
{
    public class ExternalLoginController : Controller
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;

        public ExternalLoginController(SignInManager<IdentityUser> signInManager, UserManager<IdentityUser> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }

        public async Task Login()
        {
            await HttpContext.ChallengeAsync(GoogleDefaults.AuthenticationScheme, new AuthenticationProperties()
            {
                RedirectUri = Url.Action("LoginGoogle")
            });
        }

        public async Task<IActionResult> LoginGoogle()
        {
            var authResult = await HttpContext.AuthenticateAsync(GoogleDefaults.AuthenticationScheme);
            var claimsPrincipal = authResult.Principal;
            var emailClaim = claimsPrincipal.FindFirst(ClaimTypes.Email);
            var userEmail = emailClaim?.Value;
            var userNameClaim = claimsPrincipal.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/givenname");
            var userName = userNameClaim?.Value;
            if (userEmail == null)
            {
                // Xử lý lỗi
                return RedirectToAction("Index", "Login");
            }
            // Các thông tin khác mà bạn muốn lấy
            // Kiểm tra xem tài khoản đã tồn tại hay chưa
            var user = await _userManager.FindByEmailAsync(userEmail);

            if (user == null)
            {
                // Tạo mới tài khoản nhưng không xác nhận email
                user = new IdentityUser { UserName = userName, Email = userEmail };
                var result = await _userManager.CreateAsync(user);

                if (result.Succeeded)
                {
                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    await _userManager.ConfirmEmailAsync(user, code);

                    // Đăng nhập tài khoản mới tạo
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToAction("Index", "Home"); // Chuyển hướng sau khi đăng nhập thành công
                }
                else
                {
                    // Xử lý lỗi tạo mới tài khoản
                    // ...
                }
            }
            else
            {
                // Kiểm tra xem tài khoản đã xác nhận email chưa
                var emailVerifiedClaim = await _userManager.IsEmailConfirmedAsync(user);

                if (emailVerifiedClaim)
                {
                    // Đăng nhập với tài khoản đã tồn tại và đã xác nhận email
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToAction("Index", "Home"); ; // Chuyển hướng sau khi đăng nhập thành công
                }
                else
                {
                    // Tài khoản đã tồn tại nhưng chưa xác nhận email, bạn có thể gửi lại email xác nhận ở đây
                    // ...

                    // Sau khi gửi lại email xác nhận, bạn có thể gọi ConfirmEmailAsync để xác nhận email
                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    var result = await _userManager.ConfirmEmailAsync(user, code);

                    if (result.Succeeded)
                    {
                        // Xác nhận email thành công
                        // Đăng nhập tài khoản
                        await _signInManager.SignInAsync(user, isPersistent: false);
                        return RedirectToAction("Index", "Home");  // Chuyển hướng sau khi đăng nhập thành công
                    }
                
                }
            }
            return RedirectToAction("Index", "Login");
        }


    }
}
