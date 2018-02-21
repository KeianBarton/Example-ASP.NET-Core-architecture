using Library.Domain.Entities;
using Library.Persistence;
using Library.Persistence.Exceptions;
using Library.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Library.IntegrationTests.Persistence.Repositories
{
    [TestFixture]
    public class BookRepositoryTests
    {
        private BookRepository _bookRepository;
        private ApplicationDbContext _context;
        private UnitOfWork _unitOfWork;
        private IDbContextTransaction _transaction;

        [SetUp]
        public void SetUp()
        {
            var factory = new ApplicationDbContextFactory();
            _context = factory.CreateDbContext(null);
            _bookRepository = new BookRepository(_context);
            _unitOfWork = new UnitOfWork(_context);
            _transaction = _context.Database.BeginTransaction();
        }

        [TearDown]
        public void TearDown()
        {
            _transaction.Rollback();
            _transaction = null;
            _context = null;
            _bookRepository = null;
        }

        [Test]
        public void AddBookForAuthor_WhenCalledWithInvalidArgument_ShouldThrowException()
        {
            // Act / Assert
            Assert.Throws<ArgumentNullException>(
                () => _bookRepository.AddBookForAuthor(new Guid(), null));
        }

        [Test]
        public void AddBookForAuthor_WhenCalled_ShouldAddBookToAuthorInDatabase()
        {
            // Arrange
            var author = new Author
            {
                Id = new Guid(),
                Books = new List<Book>(),
                DateOfBirth = new DateTimeOffset(),
                FirstName = "John",
                LastName = "Smith",
                Genre = "Adventure"
            };
            var book = new Book
            {
                Id = new Guid(),
                Title = "Test Book",
                Description = "Descriptive text"
            };
            var numberOfBooksBeforeChanges = _context.Books.Count();
            _context.Authors.Add(author);
            _context.SaveChanges();

            // Act
            _bookRepository.AddBookForAuthor(author.Id, book);
            _unitOfWork.Complete();

            // Assert
            var books = _context.Books.ToList();
            var authorBooks = _context.Authors.Include(a => a.Books)
                .Single(a => a.Id == author.Id).Books.ToList();
            Assert.That(books, Has.Count.EqualTo(numberOfBooksBeforeChanges + 1));
            Assert.That(authorBooks, Has.Count.EqualTo(1));
        }

        [Test]
        public void AddBookForAuthor_WhenArgumentIsInvalid_ShouldThrowException()
        {
            // Act / Assert
            Assert.Throws<ArgumentNullException>(
                () => _bookRepository.AddBookForAuthor(new Guid(), null));
        }

        [Test]
        public void AddBookForAuthor_WhenAuthorDoesNotExist_ShouldThrowException()
        {
            // Act / Assert
            Assert.Throws<DataNotFoundException>(
                () => _bookRepository.AddBookForAuthor(new Guid(), new Book()));
        }


        [Test]
        public void AddBookForAuthor_WhenAuthorAlreadyHasBook_ShouldThrowException()
        {
            // Arrange
            var book = new Book
            {
                Id = new Guid(),
                Title = "Test Book",
                Description = "Descriptive text"
            };
            var author = new Author
            {
                Id = new Guid(),
                Books = new List<Book>() { book },
                DateOfBirth = new DateTimeOffset(),
                FirstName = "John",
                LastName = "Smith",
                Genre = "Adventure"
            };
            _context.Authors.Add(author);
            _context.SaveChanges();

            // Act / Assert
            Assert.Throws<DataAlreadyExistsException>(
                () => _bookRepository.AddBookForAuthor(author.Id, book));
        }

        [Test]
        public void DeleteBook_WhenBookExists_ShouldDeleteBookFromDatabase()
        {
            // Arrange
            var book = new Book
            {
                Id = new Guid(),
                Title = "Test Book",
                Description = "Descriptive text"
            };
            _context.Books.Add(book);
            _context.SaveChanges();

            // Act
            _bookRepository.DeleteBook(book.Id);
            _unitOfWork.Complete();

            // Assert
            var result = _context.Books.ToList();
            Assert.That(result, Has.Count.EqualTo(0));
        }

        [Test]
        public void DeleteBook_WhenBookNotInDatabase_ShouldThrowException()
        {
            // Arrange
            var book = new Book
            {
                Id = new Guid(),
                Title = "Test Book",
                Description = "Descriptive text"
            };

            // Act / Assert
            Assert.Throws<DataNotFoundException>(
                () => _bookRepository.DeleteBook(book.Id));
        }

        [Test]
        public void GetBookForAuthor_WhenBookExists_ShouldGetBookFromDatabase()
        {
            // Arrange
            var book = new Book
            {
                Id = new Guid(),
                Title = "Test Book",
                Description = "Descriptive text"
            };
            var author = new Author
            {
                Id = new Guid(),
                Books = new List<Book>() { book },
                DateOfBirth = new DateTimeOffset(),
                FirstName = "John",
                LastName = "Smith",
                Genre = "Adventure"
            };
            _context.Authors.Add(author);
            _unitOfWork.Complete();

            // Act
            var result = _bookRepository.GetBookForAuthor(author.Id, book.Id);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<Book>(result);
        }

        [Test]
        public void GetBookForAuthor_WhenAuthorDoesNotExist_ShouldThrowException()
        {
            // Act / Assert
            Assert.Throws<DataNotFoundException>(
                () => _bookRepository.GetBookForAuthor(new Guid(), new Guid()));
        }

        [Test]
        public void GetBookForAuthor_WhenAuthorDoesNotHaveBookInDatabase_ShouldThrowException()
        {
            // Arrange
            var author = new Author
            {
                Id = new Guid(),
                Books = new List<Book>(),
                DateOfBirth = new DateTimeOffset(),
                FirstName = "John",
                LastName = "Smith",
                Genre = "Adventure"
            };
            _context.Authors.Add(author);
            _context.SaveChanges();

            // Act / Assert
            Assert.Throws<DataNotFoundException>(
                () => _bookRepository.GetBookForAuthor(author.Id, new Guid()));
        }

        [Test]
        public void GetBooksForAuthor_WhenAuthorsBooksExist_ShouldBeTakenFromDatabase()
        {
            // Arrange
            var book1 = new Book
            {
                Id = new Guid(),
                Title = "Test Book",
                Description = "Descriptive text"
            };
            var book2 = new Book
            {
                Id = new Guid(),
                Title = "Test Book 2",
                Description = "Descriptive text 2"
            };
            var author = new Author
            {
                Id = new Guid(),
                Books = new List<Book>() { book1, book2 },
                DateOfBirth = new DateTimeOffset(),
                FirstName = "John",
                LastName = "Smith",
                Genre = "Adventure"
            };
            _context.Authors.Add(author);
            _context.SaveChanges();

            // Act
            var result = _bookRepository.GetBooksForAuthor(author.Id);

            // Assert
            Assert.That(result, Has.Count.EqualTo(2));
        }

        [Test]
        public void GetBooksForAuthor_WhenAuthorDoesNotExist_ShouldThrowException()
        {
            // Act / Assert
            Assert.Throws<DataNotFoundException>(
                () => _bookRepository.GetBooksForAuthor(new Guid()));
        }

        [Test]
        public void GetBooksForAuthor_WhenAuthorHasNoBooksInDatabase_ShouldThrowException()
        {
            // Arrange
            var author = new Author
            {
                Id = new Guid(),
                Books = new List<Book>(),
                DateOfBirth = new DateTimeOffset(),
                FirstName = "John",
                LastName = "Smith",
                Genre = "Adventure"
            };
            _context.Authors.Add(author);
            _context.SaveChanges();

            // Act / Assert
            Assert.Throws<DataNotFoundException>(
                () => _bookRepository.GetBooksForAuthor(author.Id));
        }

        [Test]
        public void UpdateBookForAuthor_WhenCalledWithInvalidArgument_ShouldThrowException()
        {
            // Act / Assert
            Assert.Throws<ArgumentNullException>(
                () => _bookRepository.UpdateBookForAuthor(new Guid(), null));
        }

        [Test]
        public void UpdateBookForAuthor_WhenAuthorDoesNotExist_ShouldThrowException()
        {
            // Act / Assert
            Assert.Throws<DataNotFoundException>(
                () => _bookRepository.UpdateBookForAuthor(new Guid(), new Book()));
        }

        [Test]
        public void UpdateBookForAuthor_WhenBookDoesNotExist_ShouldThrowException()
        {
            // Arrange
            var author = new Author
            {
                Id = new Guid(),
                Books = new List<Book>(),
                DateOfBirth = new DateTimeOffset(),
                FirstName = "John",
                LastName = "Smith",
                Genre = "Adventure"
            };
            _context.Authors.Add(author);
            _context.SaveChanges();

            // Act / Assert
            Assert.Throws<DataNotFoundException>(
                () => _bookRepository.UpdateBookForAuthor(author.Id, new Book()));
        }

        [Test]
        public void UpdateBookForAuthor_WhenCalled_ShouldUpdateBookForAuthorInDatabase()
        {
            // Arrange
            var book = new Book
            {
                Id = new Guid(),
                Title = "Test Book",
                Description = "Descriptive text"
            };
            var author = new Author
            {
                Id = new Guid(),
                Books = new List<Book>() { book },
                DateOfBirth = new DateTimeOffset(),
                FirstName = "John",
                LastName = "Smith",
                Genre = "Adventure"
            };
            _context.Authors.Add(author);

            var newTitle = "New test book";
            var newDescription = "New description";
            book.Title = newTitle;
            book.Description = newDescription;

            _context.SaveChanges();

            // Act
            _bookRepository.UpdateBookForAuthor(author.Id, book);
            _unitOfWork.Complete();

            // Assert
            Assert.AreEqual(newTitle, author.Books.First().Title);
            Assert.AreEqual(newDescription, author.Books.First().Description);
        }

        [Test]
        public void UpdateBookForAuthor_WhenCalled_ShouldDisallowIdChanges()
        {
            // Arrange
            var book = new Book
            {
                Id = new Guid(),
                Title = "Test Book",
                Description = "Descriptive text"
            };
            var bookWithNewId = new Book
            {
                Id = Guid.NewGuid(),
                Title = book.Title,
                Description = book.Description
            };
            var author = new Author
            {
                Id = new Guid(),
                Books = new List<Book>() { book },
                DateOfBirth = new DateTimeOffset(),
                FirstName = "John",
                LastName = "Smith",
                Genre = "Adventure"
            };
            _context.Authors.Add(author);
            _context.SaveChanges();

            // Act / Assert
            Assert.Throws<DataCannotChangeIdException>(
                () => _bookRepository.UpdateBookForAuthor(author.Id, bookWithNewId));
        }
    }
}
