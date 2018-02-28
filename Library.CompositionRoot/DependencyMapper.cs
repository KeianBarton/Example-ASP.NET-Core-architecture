using AutoMapper;
using Library.EntityFramework.DatabaseContext;
using Library.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Library.CompositionRoot
{
    public class DependencyMapper
    {
        public static void SetDependencies(IServiceCollection services,
            IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            services
                .AddEntityFrameworkSqlServer()
                .AddDbContext<ApplicationDbContext>(options =>
                    options.UseSqlServer(connectionString));

            services.AddAutoMapper();
            services.AddTransient<IAuthorService, AuthorService>();
            services.AddTransient<IBookService, BookService>();
            services.AddTransient<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IApplicationDbContext, ApplicationDbContext>();
        }
    }
}