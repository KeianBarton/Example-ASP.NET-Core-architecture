using Library.Domain.Persistence.Repositories;

namespace Library.Domain.Persistence
{
    public interface IUnitOfWork
    {
        IAuthorRepository Authors { get; }
        IBookRepository Books { get; }

        void Complete();
    }
}
