using System.Configuration;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Poced.Repository.Entities;

namespace Poced.Repository.Contexts
{
    public class PocedDbContext : IdentityDbContext
    {
        // todo: Call base class
        // todo: Get connection string from appsetting.json (IOptions?)
        // todo:    services.AddDbContext<SchoolContext>(options =>
        //     options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

        public PocedDbContext(DbContextOptions<PocedDbContext> options) : base (options)
        {
            
        }
        public DbSet<Article> Articles { get; set; }
    }
}
