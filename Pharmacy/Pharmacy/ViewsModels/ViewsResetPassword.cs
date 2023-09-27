using System.ComponentModel.DataAnnotations;

namespace Pharmacy.ViewsModels
{
    public class ViewsResetPassword
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required(ErrorMessage = "Không được để trống mật khẩu")]
        [StringLength(100, ErrorMessage = "{0} dài {2} đến {1} ký tự.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        
        public string Password { get; set; }

     
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Mật khẩu phải giống nhau.")]
        public string ConfirmPassword { get; set; }

        public string Code { get; set; }
    }
}
