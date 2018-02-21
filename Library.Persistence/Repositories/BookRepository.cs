using Library.Domain.Entities;
using Library.Domain.Persistence;
using Library.Domain.Persistence.Repositories;
using Library.Persistence.Exceptions;
using Microsoft.EntityFrameworkCore;
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
            if (book == null)
                throw new ArgumentNullException();
            var author = _context.Authors
                .Include(a => a.Books)
                .SingleOrDefault(a => a.Id == authorId);
            if (author == null)
                throw new DataNotFoundException();
            if (author.Books.Any(b => b.Title == book.Title))
                throw new DataAlreadyExistsException();
            author.Books.Add(book);
        }

        public void DeleteBook(Guid bookId)
        {
            var book = _context.Books.SingleOrDefault(b => b.Id == bookId);
            if (book == null)
                throw new DataNotFoundException();
            _context.Books.Remove(book);
        }

        public Book GetBookForAuthor(Guid authorId, Guid bookId)
        {
            var author = _context.Authors
                .Include(a => a.Books)
                .SingleOrDefault(a => a.Id == authorId);
            if (author == null)
                throw new DataNotFoundException();
            var book = author.Books.SingleOrDefault(b => b.Id == bookId);
            if (book == null)
                throw new DataNotFoundException();
            return book;
        }

        public IEnumerable<Book> GetBooksForAuthor(Guid authorId)
        {
            var author = _context.Authors
                .Include(a => a.Books)
                .SingleOrDefault(a => a.Id == authorId);
            if (author == null)
                throw new DataNotFoundException();
            if (!author.Books.Any())
                throw new DataNotFoundException();
            return author.Books;
        }

        public void UpdateBookForAuthor(Guid authorId, Book book)
        {
            if (book == null)
                throw new ArgumentNullException();
            var author = _context.Authors.Include(a => a.Books)
                .SingleOrDefault(a => a.Id == authorId);
            if (author == null)
                throw new DataNotFoundException();
            var bookDb = _context.Books.SingleOrDefault(b =>
                b.Id == book.Id || b.Title == book.Title);
            if (bookDb == null)
                throw new DataNotFoundException();
            if (book.Id != bookDb.Id)
                throw new DataCannotChangeIdException();
            bookDb.Modify(book.Title, book.Description);
        }
    }
}
