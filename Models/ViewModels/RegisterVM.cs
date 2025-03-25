﻿using IdentityProject.Tools;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace IdentityProject.Models.ViewModels
{
    public class RegisterVM
    {
        [Required]
        [RegularExpression(@"^[a-zA-Z]+[0-9._!@#$%&*^-]*$", 
            ErrorMessage = "Username must start with letters and can be followed by optional digits or special characters: . _ ! @ # $ % & * ^ -")]
		[Remote(action: "IsAnyUserName", controller: "Auth",
            AdditionalFields = "__RequestVerificationToken", HttpMethod = "Post")]
		public string UserName { get; set; }

        [Required]
        [Remote(action: "IsAgeValid", controller: "Auth",
            AdditionalFields = "__RequestVerificationToken", HttpMethod = "Post")]
		public int Age { get; set; }

        [Required]
        [EmailAddress]
        [EmailValidation]
		[Remote(action: "IsAnyEmail", controller: "Auth",
			AdditionalFields = "__RequestVerificationToken", HttpMethod = "Post")]
		public string Email { get; set; }

        [Required]
        [Phone]
		[Remote(action: "IsAnyPhoneNumber", controller: "Auth",
	        AdditionalFields = "__RequestVerificationToken", HttpMethod = "Post")]
		public string PhoneNumber { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Compare(nameof(Password))]
        public string RePassword { get; set; }


    }
}
