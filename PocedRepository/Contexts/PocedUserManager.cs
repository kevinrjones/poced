using IdentityShared;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using PocedRepository.Entities;

namespace PocedRepository.Contexts
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