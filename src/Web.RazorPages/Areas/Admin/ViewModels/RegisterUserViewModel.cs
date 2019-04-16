using System.ComponentModel.DataAnnotations;

namespace StartupCreativeAgency.Web.RazorPages.Areas.Admin.ViewModels
{
    public class RegisterUserViewModel
    {
        [Required]
        [Display(Name = "User Name")]
        [StringLength(20)]
        public string UserName { get; set; }

        [Required]
        [Display(Name = "E-mail")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [StringLength(20)]
        public string Password { get; set; }

        [Required]
        [Display(Name = "Confirm Password")]
        [DataType(DataType.Password)]
        [StringLength(20)]
        [Compare(nameof(Password))]
        public string ConfirmPassword { get; set; }

        [Required(AllowEmptyStrings = false)]
        public string Role { get; set; }
    }
}
