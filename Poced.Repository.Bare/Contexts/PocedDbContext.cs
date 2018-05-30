using IdentityExpress.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Poced.Identity.Shared;
using Poced.Repository.Bare.Entities;

namespace Poced.Repository.Bare.Contexts
{
    public class PocedDbContext : IdentityExpressDbContext<PocedUser>
    {

        public PocedDbContext(DbContextOptions<PocedDbContext> options) : base (options)
        {
            
        }
        public DbSet<Article> Articles { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);
        }

    }

    public class PocedContextFactory : IDesignTimeDbContextFactory<PocedDbContext>
    {
        public PocedDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<PocedDbContext>();
            optionsBuilder.UseSqlServer("data source=.;initial catalog=Poced;integrated security=True;MultipleActiveResultSets=True");

            return new PocedDbContext(optionsBuilder.Options);
        }
    }

}
