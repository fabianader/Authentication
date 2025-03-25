using Microsoft.AspNetCore.Identity;

namespace IdentityProject.Models.AppCustomEntities
{
    public class ApplicationRoleClaim : IdentityRoleClaim<string>
    {
        public virtual ApplicationRole Role { get; set; }
    }
}