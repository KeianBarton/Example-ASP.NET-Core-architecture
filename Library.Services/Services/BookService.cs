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

        public void AddBookForAuthor(Guid authorId, Book book)
        {
            if (book == null)
                throw new ArgumentNullException();

            var author = _unitOfWork.Authors.Read(authorId);
            if (author == null)
                throw new DataNotFoundException();

            if (author.Books.Any(b => b.Title == book.Title))
                throw new DataAlreadyExistsException();

            author.Books.Add(book);
            author.Modify(author.FirstName, author.LastName, author.DateOfBirth,
                author.Genre, author.Books);
            _unitOfWork.Complete();
        }

        public bool BookExists(Guid bookId)
        {
            return _unitOfWork.Books.Read(bookId) != null;
        }

        public void DeleteBook(Guid bookId)
        {
            if (!BookExists(bookId))
                throw new DataNotFoundException();

            _unitOfWork.Books.Delete(bookId);
            _unitOfWork.Complete();
        }

        public Book GetBookForAuthor(Guid authorId, Guid bookId)
        {
            var author = _unitOfWork.Authors.Read(authorId);

            if (author == null)
                throw new DataNotFoundException();

            var book = author.Books.SingleOrDefault(b => b.Id == bookId);
            if (book == null)
                throw new DataNotFoundException();

            return book;
        }

        public IEnumerable<Book> GetBooksForAuthor(Guid authorId)
        {
            var author = _unitOfWork.Authors.Read(authorId);

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

            var author = _unitOfWork.Authors.Read(authorId);

            if (author == null)
                throw new DataNotFoundException();

            var bookDb = author.Books.SingleOrDefault(b => b.Id == book.Id);
            if (bookDb == null)
                throw new DataNotFoundException();

            bookDb.Modify(book.Title, book.Description);
            _unitOfWork.Complete();
        }
    }
}
