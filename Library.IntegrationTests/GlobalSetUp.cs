using Library.Persistence;
using Library.Persistence.Seeding;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

[SetUpFixture]
public class GlobalSetUp
{
    [OneTimeSetUp]
    public void SetUp()
    {
        var dbContextFactory = new ApplicationDbContextFactory();
        using (var dbContext = dbContextFactory.CreateDbContext(null))
        {
            MigrateDbToLatestVersion(dbContext);
            Seed(dbContext);
        }
    }

    private static void MigrateDbToLatestVersion(ApplicationDbContext context)
    {
        var migrator = context.GetInfrastructure().GetRequiredService<IMigrator>();
        migrator.Migrate();
    }

    public void Seed(ApplicationDbContext context)
    {
        // Add any extra seeding you want across all integration tests here
        context.EnsureSeedDataForContext();
    }
}
