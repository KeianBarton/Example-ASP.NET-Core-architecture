using Library.Domain.Entities;
using Library.EntityFramework.DatabaseContext;
using Library.EntityFramework.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Library.Services
{
    public class BookService : IBookService
    {
        private readonly IUnitOfWork _unitOfWork;

        public BookService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public Guid AddBookForAuthor(Guid authorId, Book book)
        {
            if (book == null)
                throw new ArgumentNullException();

            var author = _unitOfWork.Authors.Read(authorId);
            if (author == null)
                throw new DataNotFoundException("Author not found");

            if (author.Books.Any(b => b.Title == book.Title))
                throw new DataAlreadyExistsException("Book already exists");

            author.Books.Add(book);
            _unitOfWork.Complete();
            return book.Id;
        }

        public bool BookExists(Guid bookId)
        {
            return _unitOfWork.Books.Read(bookId) != null;
        }

        public void DeleteBook(Guid bookId)
        {
            if (!BookExists(bookId))
                throw new DataNotFoundException("Book not found");

            _unitOfWork.Books.Delete(bookId);
            _unitOfWork.Complete();
        }

        public Book GetBookForAuthor(Guid authorId, Guid bookId)
        {
            var author = _unitOfWork.Authors.Read(authorId);

            if (author == null)
                throw new DataNotFoundException("Author not found");

            var book = author.Books.SingleOrDefault(b => b.Id == bookId);
            if (book == null)
                throw new DataNotFoundException("Book not found");

            return book;
        }

        public IEnumerable<Book> GetBooksForAuthor(Guid authorId)
        {
            var author = _unitOfWork.Authors.Read(authorId);

            if (author == null)
                throw new DataNotFoundException("Author not found");

            if (!author.Books.Any())
                throw new DataNotFoundException("No books found");

            return author.Books;
        }

        public void UpdateBookForAuthor(Guid authorId, Book book)
        {
            if (book == null)
                throw new ArgumentNullException();

            var author = _unitOfWork.Authors.Read(authorId);

            if (author == null)
                throw new DataNotFoundException("Author not found");

            var bookDb = author.Books.SingleOrDefault(b => b.Id == book.Id);
            if (bookDb == null)
                throw new DataNotFoundException("Book not found");

            bookDb.Modify(book.Title, book.Description);
            _unitOfWork.Complete();
        }
    }
}