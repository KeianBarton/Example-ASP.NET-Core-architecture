using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace Library.EntityFramework.DatabaseContext
{
    public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        public ApplicationDbContext CreateDbContext(string[] args)
        {
            // TODO: To get this working with integration tests, you have to place the
            // appsettings.json file for Persistence in bin/debug
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();
            var connectionString = configuration.GetConnectionString("DefaultConnection");

            var builder = new DbContextOptionsBuilder<ApplicationDbContext>();
            builder.UseSqlServer(connectionString);

            return new ApplicationDbContext(builder.Options);
        }
    }
}