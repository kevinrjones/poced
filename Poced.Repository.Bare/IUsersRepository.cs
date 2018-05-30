using System.Collections.Generic;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Poced.Identity.Shared;
using Poced.RepositoryInterfaces;

namespace Poced.Repository.Bare
{
    public interface IUsersRepository : IRepository<PocedUser>
    {
        PocedUser Create(string username);
        PocedUser Create(string userName, string password);
        IdentityResult CreateIdentity(string provider, string providerId, string authenticationType);
        bool AddLogin(string userId, string provider, string providerId);
        bool AddClaim(string userId, Claim claim);
        IdentityResult CreateIdentity(PocedUser PocedUser, string authenticationName);
        PocedUser FindByName(string userName);
        IList<Claim> GetClaims(string userId);
        bool RemoveClaim(string userId, Claim claim);
    }
}