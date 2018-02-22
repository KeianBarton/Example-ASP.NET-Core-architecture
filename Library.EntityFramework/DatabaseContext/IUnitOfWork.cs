using Library.EntityFramework.Repositories;

namespace Library.EntityFramework.DatabaseContext
{
    public interface IUnitOfWork
    {
        IAuthorRepository Authors { get; }
        IBookRepository Books { get; }

        void Complete();
    }
}
