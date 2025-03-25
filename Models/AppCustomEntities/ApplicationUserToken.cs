using Microsoft.AspNetCore.Identity;

namespace IdentityProject.Models.AppCustomEntities
{
    public class ApplicationUserToken : IdentityUserToken<string>
    {
        public virtual ApplicationUser User { get; set; }
    }
}