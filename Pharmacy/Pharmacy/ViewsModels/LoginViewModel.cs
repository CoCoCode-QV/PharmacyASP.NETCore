using System.ComponentModel.DataAnnotations;
using System.Security.Permissions;

namespace Pharmacy.ViewsModels
{
    public class LoginViewModel
    {
        [Display(Name ="Tên đăng nhập ")]
        public string Username {  get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
