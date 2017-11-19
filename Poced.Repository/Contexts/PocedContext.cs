using System.Configuration;
using System.Data.Entity;
using Microsoft.AspNet.Identity.EntityFramework;
using Poced.Repository.Entities;

namespace Poced.Repository.Contexts
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
