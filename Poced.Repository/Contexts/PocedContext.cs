using System.Configuration;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Poced.Repository.Entities;

namespace Poced.Repository.Contexts
{
    class PocedContext : IdentityDbContext
    {
        // todo: Call base class
        // todo: Get connection string from appsetting.json (IOptions?)
        // todo:    services.AddDbContext<SchoolContext>(options =>
        //     options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));


        public PocedContext() : this("")
        {
            
        }
        public PocedContext(string connectionString) 
        {

        }

        public PocedContext(DbContextOptions<PocedContext> options) : base (options)
        {
            
        }
        public DbSet<Article> Articles { get; set; }
    }
}
