using System.Collections.Generic;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Poced.Identity.Shared;
using Poced.Repository.Contexts;

namespace Poced.Repository
{
    public class UsersRepository : PocedRepository<PocedUser>, IUsersRepository
    {
        private readonly PocedUserManager pocedUserManager;

        public UsersRepository(PocedUserManager userManager, PocedDbContext dbContext) : base(dbContext)
        {
            pocedUserManager = userManager;
        }

        public bool AddLogin(string userId, string provider, string providerId)
        {
            var pocedUser = new PocedUser { Id = userId };
            var result = pocedUserManager.AddLoginAsync(pocedUser, new UserLoginInfo(provider, providerId, "")).Result;
            return result == IdentityResult.Success;
        }

        public bool AddClaim(string userId, Claim claim)
        {
            var pocedUser = new PocedUser{Id = userId};
            var result = pocedUserManager.AddClaimAsync(pocedUser, claim).Result;
            return result == IdentityResult.Success;
        }

        public PocedUser Create(string userName)
        {
            var user = new PocedUser {UserName = userName};

            var result = pocedUserManager.CreateAsync(user).Result;
            return result != IdentityResult.Success ? null : user;
        }

        public PocedUser Create(string userName, string password)
        {
            var user = new PocedUser {UserName = userName};
            var result = pocedUserManager.CreateAsync(user, password).Result;
            return result != IdentityResult.Success ? null : user;
        }

        public IdentityResult CreateIdentity(string provider, string providerId, string authenticationType)
        {
            var user = pocedUserManager.FindByLoginAsync(provider, providerId).Result;
            if (user != null)
            {
                IdentityResult claims = pocedUserManager.CreateAsync(user, authenticationType).Result;
                if (claims != null) return claims;
            }
            return null;
        }

        public IdentityResult CreateIdentity(PocedUser pocedUser, string authenticationName)
        {
            return pocedUserManager.CreateAsync(pocedUser, authenticationName).Result;
        }

        public PocedUser FindByName(string userName)
        {
            return pocedUserManager.FindByNameAsync(userName).Result;
        }

        public IList<Claim> GetClaims(string userId)
        {
            var pocedUser = new PocedUser { Id = userId };
            return pocedUserManager.GetClaimsAsync(pocedUser).Result;
        }

        public bool RemoveClaim(string userId, Claim claim)
        {
            var pocedUser = new PocedUser { Id = userId };
            var result = pocedUserManager.RemoveClaimAsync(pocedUser, claim).Result;
            return result == IdentityResult.Success;
        }
    }
}