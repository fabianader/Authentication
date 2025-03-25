using Microsoft.AspNetCore.Identity;

namespace IdentityProject.Models.AppCustomEntities
{
    public class ApplicationUserLogin : IdentityUserLogin<string>
    {
        public virtual ApplicationUser User { get; set; }
    }
}