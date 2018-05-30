using System.Configuration;
using IdentityExpress.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Poced.Identity.Shared;
using Poced.Repository.Entities;

namespace Poced.Repository.Contexts
{
    public class PocedDbContext : IdentityDbContext<PocedUser>
    {

        public PocedDbContext(DbContextOptions<PocedDbContext> options) : base(options)
        {

        }
        public DbSet<Article> Articles { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {

            // todo: this issue?: https://github.com/aspnet/EntityFrameworkCore/issues/4987
            //builder.Entity<IdentityUser>().Ignore(b => b.Id);
            //builder.Entity<IdentityExpressUser>().HasKey(b => b.Id);
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
            optionsBuilder.UseSqlServer("data source=.;initial catalog=PocedDb;integrated security=True;MultipleActiveResultSets=True");

            return new PocedDbContext(optionsBuilder.Options);
        }
    }

}
