using System.ComponentModel.DataAnnotations;

namespace IdentityProject.Models.ViewModels
{
    public class ResetPasswordVM
    {
        [Required]
        [RegularExpression(@"^[a-zA-Z]+[0-9._!@#$%&*^-]*$", 
            ErrorMessage = "Username must start with letters and can be followed by optional digits or special characters: . _ ! @ # $ % & * ^ -")]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Compare(nameof(NewPassword))]
        public string ConfirmNewPassword { get; set; }

        [Required]
        public string Token { get; set; }
    }
}
