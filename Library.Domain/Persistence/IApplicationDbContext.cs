using Library.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Library.Domain.Persistence
{
    public interface IApplicationDbContext
    {
        DbSet<Author> Authors { get; set; }
        DbSet<Book> Books { get; set; }
    }
}
