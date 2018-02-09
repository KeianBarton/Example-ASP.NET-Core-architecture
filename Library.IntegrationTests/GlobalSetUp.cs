using Library.Persistence;
using Library.Persistence.Seeding;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace GigHub.IntegrationTests
{
    [SetUpFixture]
    public class GlobalSetUp
    {
        [OneTimeSetUp]
        public void SetUp()
        {
            MigrateDbToLatestVersion();
            Seed();
        }

        private static void MigrateDbToLatestVersion()
        {
            using (var db = new ApplicationDbContext())
            {
                var migrator = db.GetInfrastructure().GetRequiredService<IMigrator>();
                migrator.Migrate();
            }
        }

        public void Seed()
        {
            // Add any extra seeding you want across all integration tests here
            var context = new ApplicationDbContext();
            context.EnsureSeedDataForContext();
        }
    }
}
