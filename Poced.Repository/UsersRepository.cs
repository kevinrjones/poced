using System.Collections.Generic;
using System.Security.Claims;
using Microsoft.AspNet.Identity;
using Poced.Identity.Shared;
using Poced.Repository.Contexts;

namespace Poced.Repository
{
    public class UsersRepository : PocedRepository<PocedUser>, IUsersRepository
    {
        private readonly PocedUserManager pocedUserManager;

        public UsersRepository(string connectionString) : base(connectionString)
        {
            pocedUserManager = new PocedUserManager(connectionString);
        }

        public bool AddLogin(string userId, string provider, string providerId)
        {
            var result = pocedUserManager.AddLogin(userId, new UserLoginInfo(provider, providerId));
            return result == IdentityResult.Success;
        }

        public bool AddClaim(string userId, Claim claim)
        {
            var result = pocedUserManager.AddClaim(userId, claim);
            return result == IdentityResult.Success;
        }

        public PocedUser Create(string userName)
        {
            var user = new PocedUser {UserName = userName};

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
                var claims = pocedUserManager.CreateIdentity(user, authenticationType);
                if (claims != null) return claims;
            }
            return null;
        }

        public ClaimsIdentity CreateIdentity(PocedUser pocedUser, string authenticationName)
        {
            return pocedUserManager.CreateIdentity(pocedUser, authenticationName);
        }

        public PocedUser FindByName(string userName)
        {
            return pocedUserManager.FindByName(userName);
        }

        public IList<Claim> GetClaims(string userId)
        {
            return pocedUserManager.GetClaims(userId);
        }

        public bool RemoveClaim(string userId, Claim claim)
        {
            var result = pocedUserManager.RemoveClaim(userId, claim);
            return result == IdentityResult.Success;
        }
    }
}