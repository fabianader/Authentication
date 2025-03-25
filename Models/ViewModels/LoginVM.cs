using Microsoft.AspNetCore.Authentication;
using System.ComponentModel.DataAnnotations;

namespace IdentityProject.Models.ViewModels
{
    public class LoginVM
    {
        [Required]
        [RegularExpression(@"^[a-zA-Z]+[0-9._!@#$%&*^-]*$", 
            ErrorMessage = "Username must start with letters and can be followed by optional digits or special characters: . _ ! @ # $ % & * ^ -")]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        public bool RememberMe { get; set; }


        public string ReturnUrl { get; set; }
        public List<AuthenticationScheme>? ExternalLogins { get; set;}
    }
}
