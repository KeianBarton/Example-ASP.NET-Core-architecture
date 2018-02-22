using Library.EntityFramework.Repositories;

namespace Library.EntityFramework.DatabaseContext
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;

        public IAuthorRepository Authors { get; private set; }
        public IBookRepository Books { get; private set; }

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
            Authors = new AuthorRepository(context);
            Books = new BookRepository(context);
        }

        public void Complete()
        {
            _context.SaveChanges();
        }
    }
}
