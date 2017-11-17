using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity.EntityFramework;
using PocedRepository.Entities;

namespace PocedRepository.Contexts
{
    class PocedContext : IdentityDbContext
    {
        public PocedContext() : base(ConfigurationManager.ConnectionStrings["PocedEntities"].ConnectionString)
        {
            
        }
        public PocedContext(string connectionString) : base(connectionString)
        {

        }
        public DbSet<Article> Articles { get; set; }
    }
}
