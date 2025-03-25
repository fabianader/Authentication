using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace IdentityProject.Models.ViewModels
{
    public class EmailCodeVM
    {
        [Required]
        [RegularExpression(@"^[a-zA-Z]+[0-9._!@#$%&*^-]*$", 
            ErrorMessage = "Username must start with letters and can be followed by optional digits or special characters: . _ ! @ # $ % & * ^ -")]
		public string UserName { get; set; }

        [Required]
        public string Token { get; set; }

        [Required]
        public string Code { get; set; }    
    }
}
