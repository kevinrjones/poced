using System.Collections.Generic;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Poced.Identity.Shared;
using Poced.Repository.Contexts;

namespace Poced.Repository
{
    public class UsersRepository : PocedRepository<PocedUser>, IUsersRepository
    {
        private readonly UserManager<PocedUser> _userManager;


        public UsersRepository(UserManager<PocedUser> userManager, PocedDbContext dbContext) : base(dbContext)
        {
            _userManager = userManager;

        }

        //public UsersRepository(PocedUserManager userManager, PocedDbContext dbContext) : base(dbContext)
        //{
        //    _userManager = userManager;
        //}

        public bool AddLogin(string userId, string provider, string providerId)
        {
            var pocedUser = new PocedUser { Id = userId };
            var result = _userManager.AddLoginAsync(pocedUser, new UserLoginInfo(provider, providerId, "")).Result;
            return result == IdentityResult.Success;
        }

        public bool AddClaim(string userId, Claim claim)
        {
            var pocedUser = new PocedUser{Id = userId};
            var result = _userManager.AddClaimAsync(pocedUser, claim).Result;
            return result == IdentityResult.Success;
        }

        public PocedUser Create(string userName)
        {
            var user = new PocedUser {UserName = userName};

            var result = _userManager.CreateAsync(user).Result;
            return result != IdentityResult.Success ? null : user;
        }

        public PocedUser Create(string userName, string password)
        {
            var user = new PocedUser {UserName = userName};
            var result = _userManager.CreateAsync(user, password).Result;
            return result != IdentityResult.Success ? null : user;
        }

        public IdentityResult CreateIdentity(string provider, string providerId, string authenticationType)
        {
            var user = _userManager.FindByLoginAsync(provider, providerId).Result;
            if (user != null)
            {
                IdentityResult claims = _userManager.CreateAsync(user, authenticationType).Result;
                if (claims != null) return claims;
            }
            return null;
        }

        public IdentityResult CreateIdentity(PocedUser pocedUser, string authenticationName)
        {
            return _userManager.CreateAsync(pocedUser, authenticationName).Result;
        }

        public PocedUser FindByName(string userName)
        {
            return _userManager.FindByNameAsync(userName).Result;
        }

        public IList<Claim> GetClaims(string userId)
        {
            var pocedUser = new PocedUser { Id = userId };
            return _userManager.GetClaimsAsync(pocedUser).Result;
        }

        public bool RemoveClaim(string userId, Claim claim)
        {
            var pocedUser = new PocedUser { Id = userId };
            var result = _userManager.RemoveClaimAsync(pocedUser, claim).Result;
            return result == IdentityResult.Success;
        }
    }
}