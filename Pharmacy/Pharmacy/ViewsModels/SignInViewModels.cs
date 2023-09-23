using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace Pharmacy.ViewsModels
{
    public class SignInViewModels
    {
        [Required]
        public string userName { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [DisplayName("Remember Me")]
        public bool RememberMe { get; set; }
    }
}
