using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pharmacy.Areas.Admin.Controllers;
using Pharmacy.ViewsModels;

namespace Pharmacy.Models
{
    public class UserModels
    {
        private readonly UserManager<IdentityUser> _userManager;


        public UserModels(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<IEnumerable<UserViewModels>> GetListUserAsync()
        {

             var UserAdmin = await _userManager.GetUsersInRoleAsync("Admin");
            var UserStaff = await _userManager.GetUsersInRoleAsync("Staff");
            var userList = UserAdmin.Concat(UserStaff).ToList();

            if (userList != null )
            {

                var userView = userList.Select(user => new UserViewModels
                {
                    UserId = user.Id,
                    Username = user.UserName,
                    Email = user.Email,
                    Role = UserAdmin.Contains(user) ? "Admin" : "Staff"
                }).ToList();

                return userView;
            }
            return null;
        }
    }
}
