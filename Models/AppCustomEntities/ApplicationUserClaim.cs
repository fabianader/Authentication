using Microsoft.AspNetCore.Identity;

namespace IdentityProject.Models.AppCustomEntities
{
    public class ApplicationUserClaim : IdentityUserClaim<string>
    {
        public virtual ApplicationUser User { get; set; }
    }
}