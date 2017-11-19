using System.Data.Entity.Migrations;
using Poced.Repository.Contexts;

namespace Poced.Repository.Migrations
{
    internal sealed class Configuration : DbMigrationsConfiguration<PocedContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
        }

        protected override void Seed(PocedContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data.
        }
    }
}
