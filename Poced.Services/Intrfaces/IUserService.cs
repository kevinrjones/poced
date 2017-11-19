using System.Collections.Generic;
using System.Security.Claims;
using Poced.Identity.Shared;

namespace Poced.Services.Intrfaces
{
    public interface IUserService
    {
        PocedUser CreateAndLoginUser(string username, string provider, string providerId, IEnumerable<Claim> claims);
        PocedUser CreateUser(string userName, string password);
        ClaimsIdentity CreateUserIdentity(string provider, string providerId);
        bool AddLogin(string userId, string provider, string providerId);
        ClaimsIdentity CreateIdentity(PocedUser user, string authenticationName);
        PocedUser FindByName(string userName);
        IList<Claim> GetClaims(string userId);
        bool RemoveClaim(string userId, Claim claim);
        bool AddClaim(string userId, Claim claim);
    }
}
