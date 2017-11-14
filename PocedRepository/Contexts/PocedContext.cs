using System;
using System.Collections.Generic;
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
        public PocedContext(string connectionString) : base(connectionString)
        {
            
        }
        public DbSet<Article> Articles { get; set; }
    }
}
