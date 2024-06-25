using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Pharmacy.Models;
using Pharmacy.ViewsModels;

namespace Pharmacy.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "SuperAdmin")]
    public class UserController : Controller
    {

        private readonly UserModels _userModels;

        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public UserController(UserModels userModels, UserManager<IdentityUser> userManager,  RoleManager<IdentityRole> roleManager )
        {
            _userModels = userModels;
            _roleManager = roleManager;
            _userManager = userManager;
        }

        public async Task<IActionResult> IndexAsync()
        {
            var UserAdmin = await _userManager.GetUsersInRoleAsync("Admin");
            var UserSuperAdmin = await _userManager.GetUsersInRoleAsync("SuperAdmin");
            var UserStaff = await _userManager.GetUsersInRoleAsync("Staff");
            var userList = UserAdmin.Concat(UserStaff).Concat(UserSuperAdmin).ToList();

            if (userList != null)
            {

                var userView = userList.Select(user => new UserViewModels
                {
                    UserId = user.Id,
                    Username = user.UserName,
                    Email = user.Email,
                    Role = UserAdmin.Contains(user) ? "Admin" : UserSuperAdmin.Contains(user) ? "SuperAdmin" : "Staff"
                }).ToList();

                return View(userView);
            }
            return NoContent();
        }
        public IActionResult Create()
        {
            var roles =  _roleManager.Roles
                 .Where(r => r.Name != "Member")
                 .ToList();
            ViewBag.ListRoles = new SelectList(roles, "Name", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UserViewModels model)
        {
            if (model.Email != null && model.Password !=null && model.Role != null)
            {
                var user = new IdentityUser { UserName = model.Email, Email = model.Email };
                user.EmailConfirmed = true;
                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, model.Role);
                    TempData["Message"] = "Thêm tài khoản thành công.";
                    return RedirectToAction("Index");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }

            // Nếu có lỗi, cần lấy lại danh sách roles để render lại view
            var roles = await _roleManager.Roles
                .Where(r => r.Name != "Member")
                .ToListAsync();

            ViewBag.ListRoles = new SelectList(roles, "Name", "Name");
            TempData["error"] = "Thêm tài khoản lỗi";
            return View(model);
        }

            public async Task<IActionResult> Delete(string id)
            {
            var user = await _userManager.FindByIdAsync(id);
            if (user != null)
            {
                var result = await _userManager.DeleteAsync(user);
                if (result.Succeeded)
                {
                    TempData["Message"] = "Tài khoản đã được xóa thành công.";
                }
                else
                {
                    TempData["Error"] = "Đã xảy ra lỗi khi xóa tài khoản.";
                }
            }
            else
            {
                TempData["Error"] = "Không tìm thấy tài khoản.";
            }
            return RedirectToAction("Index");
        }
    
    }
}
