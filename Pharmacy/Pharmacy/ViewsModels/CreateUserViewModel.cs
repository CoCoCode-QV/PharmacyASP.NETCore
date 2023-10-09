using System.ComponentModel.DataAnnotations;
using System.Security.Permissions;

namespace Pharmacy.ViewsModels
{
    public class CreateUserViewModel
    {
        [Required(ErrorMessage = "Tên tài khoản không được để trống")]
        public string UserName { get; set; }

        [EmailAddress]
        [Required(ErrorMessage ="Email không được để trống")]
        public string Email {  get; set; }

        [Required(ErrorMessage = "Mật khẩu không được để trống")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage = "Xác nhận mật khẩu không được để trống")]
        [DataType(DataType.Password)]
        [Display(Name = "Xác nhận mật khẩu")]
        [Compare("Password", ErrorMessage ="Xác nhận mật khẩu không khớp với mật khẩu")]
        public string confirmPassword {  get; set; }

    }
}
