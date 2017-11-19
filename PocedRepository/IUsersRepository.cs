﻿using System.Collections.Generic;
using System.Security.Claims;
using IdentityShared;
using Microsoft.Owin.Security;
using PocedRepository.Entities;
using Repository;

namespace PocedRepository
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