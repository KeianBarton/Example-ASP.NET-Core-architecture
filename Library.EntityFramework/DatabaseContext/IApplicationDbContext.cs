using Library.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Library.EntityFramework.DatabaseContext
{
    public interface IApplicationDbContext
    {
        DbSet<Author> Authors { get; set; }
        DbSet<Book> Books { get; set; }
    }
}