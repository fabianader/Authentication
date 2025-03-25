using IdentityProject.Models.AppCustomEntities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;

namespace IdentityProject.Tools
{
    public static class Tools
    {
        public static string CodeGenerator(int start, int end)
        {
            if (start < end)
            {
                Random random = new Random();
                int Code = random.Next(start, end);
                return Code.ToString();
            }
            else
                throw new ArgumentException("Argument Error...");
        }

        //public static async Task<IActionResult> IsAnyOf(Type parameter)
        //{
        //    ApplicationUser.Attributes
        //}

        public static string Encrypt(string token)
        {
            return WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));
        }

        public static string Decrypt(string token)
        {
            return Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(token));
        }
    }
}
