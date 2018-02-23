using Library.Domain.Entities;
using Library.EntityFramework.Repositories;

namespace Library.EntityFramework.DatabaseContext
{
    public interface IUnitOfWork
    {
        IRepository<Author> Authors { get; }

        IRepository<Book> Books { get; }

        void Complete();
    }
}