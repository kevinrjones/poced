using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Poced.Identity.Shared;
using Poced.Repository;
using Poced.Services.Intrfaces;

namespace Poced.Services.Implementations
{
    public class UserService : IUserService
    {
        private readonly IUsersRepository _usersRepository;

        public UserService(IUsersRepository usersRepository)
        {
            _usersRepository = usersRepository;
        }

        public PocedUser CreateUser(string userName, string password)
        {
            return _usersRepository.Create(userName, password);
        }

        public IdentityResult CreateUserIdentity(string provider, string providerId)
        {
            return _usersRepository.CreateIdentity(provider, providerId, "Cookie");
        }

        public bool AddLogin(string userId, string provider, string providerId)
        {
            return _usersRepository.AddLogin(userId, provider, providerId);
        }

        public IdentityResult CreateIdentity(PocedUser user, string authenticationName)
        {
            return _usersRepository.CreateIdentity(user, authenticationName);
        }

        public PocedUser FindByName(string userName)
        {
            return _usersRepository.FindByName(userName);
        }

        public IList<Claim> GetClaims(string userId)
        {
            return _usersRepository.GetClaims(userId);
        }

        public bool RemoveClaim(string userId, Claim claim)
        {
            return _usersRepository.RemoveClaim(userId, claim);
        }

        public bool AddClaim(string userId, Claim claim)
        {
            return _usersRepository.AddClaim(userId, claim);
        }

        public PocedUser CreateAndLoginUser(string username, string provider, string providerId,
            IEnumerable<Claim> claims)
        {
            var user = _usersRepository.Create(username);
            if (user != null)
            {
                var result = _usersRepository.AddLogin(user.Id, provider, providerId);
                if (result)
                {
                    var validclaims = claims.Where(x => x.Type != ClaimTypes.NameIdentifier);
                    foreach (var claim in validclaims)
                    {
                        _usersRepository.AddClaim(user.Id, claim);
                    }
                }
            }
            return user;
        }
    }
}