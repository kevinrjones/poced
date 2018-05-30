using System.Reflection;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Poced.Repository.Contexts
{
    public class PocedPersistedGrantContextFactory : IDesignTimeDbContextFactory<PersistedGrantDbContext>
    {
        public PersistedGrantDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<PersistedGrantDbContext>();
            var migrationsAssembly = typeof(PocedConfigurationContextFactory).GetTypeInfo().Assembly.GetName().Name;

            optionsBuilder.UseSqlServer(
                "data source=.;initial catalog=PocedDb;integrated security=True;MultipleActiveResultSets=True",
                sql => sql.MigrationsAssembly(migrationsAssembly));


            return new PersistedGrantDbContext(optionsBuilder.Options, new OperationalStoreOptions());
        }
    }

}