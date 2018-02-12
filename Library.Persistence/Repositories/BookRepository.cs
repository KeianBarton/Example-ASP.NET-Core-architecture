using Library.Domain.Entities;
using Library.Domain.Persistence;
using Library.Domain.Persistence.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Library.Persistence.Repositories
{
    public class BookRepository : IBookRepository
    {
        private readonly IApplicationDbContext _context;

        public BookRepository(IApplicationDbContext context)
        {
            _context = context;
        }

        public void AddBookForAuthor(Guid authorId, Book book)
        {
            throw new NotImplementedException();
        }

        public void DeleteBook(Book book)
        {
            throw new NotImplementedException();
        }

        public Book GetBookForAuthor(Guid authorId, Guid bookId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Book> GetBooksForAuthor(Guid authorId)
        {
            throw new NotImplementedException();
        }

        public void UpdateBookForAuthor(Guid authorId, Book book)
        {
            throw new NotImplementedException();
        }
    }
}
