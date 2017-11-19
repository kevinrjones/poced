using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Poced.Identity.Shared;

namespace Poced.Repository.Contexts
{
    public class PocedUserManager : UserManager<PocedUser>
    {
        public PocedUserManager(string connectionString) : base(
            new UserStore<PocedUser>(
                new PocedContext(connectionString)))
        {
        }
    }
}