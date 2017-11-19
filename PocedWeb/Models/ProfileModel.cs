using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;

namespace PocedWeb.Models
{
    public class ProfileModel
    {
        // do not remove
        public ProfileModel()
        {
            
        }
        public ProfileModel(IEnumerable<Claim> claims)
        {
            if (claims.Any(x => x.Type == ClaimTypes.GivenName))
            {
                First = claims.First(x => x.Type == ClaimTypes.GivenName).Value;
            }
            if (claims.Any(x => x.Type == ClaimTypes.Surname))
            {
                Last = claims.First(x => x.Type == ClaimTypes.Surname).Value;
            }
        }

        public string First { get; set; }
        public string Last { get; set; }
    }
}