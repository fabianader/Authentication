using IdentityProject.Tools;
using System.ComponentModel.DataAnnotations;

namespace IdentityProject.Models.ViewModels
{
    public class ForgotPasswordVM
    {
        [Required]
        [Display(Name = "User Name")]
        [RegularExpression(@"^[a-zA-Z]+[0-9._!@#$%&*^-]*$", 
            ErrorMessage = "Username must start with letters and can be followed by optional digits or special characters: . _ ! @ # $ % & * ^ -")]
        public string UserName { get; set; }

        [Required]
        [EmailAddress]
        [EmailValidation]
        public string Email { get; set; }
    }
}
