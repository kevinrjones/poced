using System.Security.Claims;
using IdentityShared;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using PocedRepository.Contexts;
using PocedRepository.Entities;

namespace PocedRepository
{
    public class UsersRepository : PocedRepository<PocedUser>, IUsersRepository
    {
        private PocedUserManager pocedUserManager;
        public UsersRepository(string connectionString) : base(connectionString)
        {
            pocedUserManager = new PocedUserManager(connectionString);
        }

        public PocedUser Create(string userName)
        {
            var user = new PocedUser { UserName = userName };

            var result = pocedUserManager.Create(user);
            return result != IdentityResult.Success ? null : user;
        }

        public PocedUser Create(string userName, string password)
        {
            var user = new PocedUser {UserName = userName};
            var result = pocedUserManager.Create(user, password);
            return result != IdentityResult.Success ? null : user;
        }

        public ClaimsIdentity CreateIdentity(string provider, string providerId, string authenticationType)
        {
            var user = pocedUserManager.Find(new UserLoginInfo(provider, providerId));
            if (user != null)
            {
                ClaimsIdentity claims = pocedUserManager.CreateIdentity(user, authenticationType);
                if (claims != null) return claims;
            }
            return null;
        }

        public bool AddLogin(string userId, string provider, string providerId)
        {
            var result = pocedUserManager.AddLogin(userId, new UserLoginInfo(provider, providerId));
            return result == IdentityResult.Success;
        }

        public void AddClaim(string userId, Claim claim)
        {
            pocedUserManager.AddClaim(userId, claim);
        }

        public ClaimsIdentity CreateIdentity(PocedUser pocedUser, string authenticationName)
        {
            return pocedUserManager.CreateIdentity(pocedUser, authenticationName);
        }
    }
}