using System.Collections.Generic;
using System.Security.Claims;
using Poced.Identity.Shared;
using Poced.RepositoryInterfaces;

namespace Poced.Repository
{
    public interface IUsersRepository : IRepository<PocedUser>
    {
        PocedUser Create(string username);
        PocedUser Create(string userName, string password);
        ClaimsIdentity CreateIdentity(string provider, string providerId, string authenticationType);
        bool AddLogin(string userId, string provider, string providerId);
        bool AddClaim(string userId, Claim claim);
        ClaimsIdentity CreateIdentity(PocedUser pocedUser, string authenticationName);
        PocedUser FindByName(string userName);
        IList<Claim> GetClaims(string userId);
        bool RemoveClaim(string userId, Claim claim);
    }
}