using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Poced.Identity.Shared;

namespace Poced.Services.Intrfaces
{
    public interface IUserService
    {
        PocedUser CreateAndLoginUser(string username, string provider, string providerId, IEnumerable<Claim> claims);

        Task<IdentityResult> CreateAsync(PocedUser user);

        PocedUser CreateUser(string userName, string password);
        IdentityResult CreateUserIdentity(string provider, string providerId);
        bool AddLogin(string userId, string provider, string providerId);
        IdentityResult CreateIdentity(PocedUser user, string authenticationName);
        PocedUser FindByName(string userName);
        IList<Claim> GetClaims(string userId);
        bool RemoveClaim(string userId, Claim claim);
        bool AddClaim(string userId, Claim claim);
        Task<SignInResult> PasswordSignInAsync(string email, string password, bool rememberMe);
        Task SignInAsync(PocedUser user, bool isPersistent);
        Task SignOutAsync();
        AuthenticationProperties ConfigureExternalAuthenticationProperties(string provider, string redirectUrl);
        Task<ExternalLoginInfo> GetExternalLoginInfoAsync();
        Task<SignInResult> ExternalLoginSignInAsync(string infoLoginProvider, string infoProviderKey, bool isPersistent, bool bypassTwoFactor);
        Task<IdentityResult> AddLoginAsync(PocedUser user, ExternalLoginInfo info);
    }
}
