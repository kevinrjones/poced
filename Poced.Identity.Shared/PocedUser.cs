using IdentityExpress.Identity;
using Microsoft.AspNetCore.Identity;

namespace Poced.Identity.Shared
{
    public class PocedUser : IdentityUser
    {
        public bool IsBlocked { get; set; }
        public bool IsDeleted { get; set; }

    }
}
