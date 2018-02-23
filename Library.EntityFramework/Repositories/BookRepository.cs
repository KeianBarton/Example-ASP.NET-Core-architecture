using Library.Domain.Entities;
using Library.EntityFramework.DatabaseContext;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Library.EntityFramework.Repositories
{
    public class BookRepository : IRepository<Book>
    {
        private readonly IApplicationDbContext _context;

        public BookRepository(IApplicationDbContext context)
        {
            _context = context;
        }

        public void Create(Book book)
        {
            _context.Books.Add(book);
        }

        public void Delete(Guid bookId)
        {
            var author = _context.Books.Single(a => a.Id == bookId);
            _context.Books.Remove(author);
        }

        public Book Read(Guid bookId)
        {
            return _context.Books.SingleOrDefault(a => a.Id == bookId);
        }

        public IEnumerable<Book> ReadAll()
        {
            return _context.Books;
        }

        public void Update(Book book)
        {
            var bookDb = _context.Books.Single(a => a.Id == book.Id);
            bookDb = book;
        }
    }
}