using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Poced.Identity.Shared;

namespace Poced.Repository.Contexts
{
    public class PocedUserManager : UserManager<PocedUser>
    {
//        public PocedUserManager(string connectionString) : base(
//            new UserStore<PocedUser>(
//                new PocedDbContext(connectionString)))
//        {
//        }


            // todo: how does this work?
        public PocedUserManager(IUserStore<PocedUser> store, IOptions<IdentityOptions> optionsAccessor, IPasswordHasher<PocedUser> passwordHasher, IEnumerable<IUserValidator<PocedUser>> userValidators, IEnumerable<IPasswordValidator<PocedUser>> passwordValidators, ILookupNormalizer keyNormalizer, IdentityErrorDescriber errors, IServiceProvider services, ILogger<UserManager<PocedUser>> logger) : base(store, optionsAccessor, passwordHasher, userValidators, passwordValidators, keyNormalizer, errors, services, logger)
        {
        }
    }
}