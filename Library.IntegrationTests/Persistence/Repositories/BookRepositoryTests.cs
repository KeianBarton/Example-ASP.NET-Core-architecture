using GigHub.IntegrationTests;
using Library.Domain.Entities;
using Library.Persistence;
using Library.Persistence.Exceptions;
using Library.Persistence.Repositories;
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

        [SetUp]
        public void SetUp()
        {
            _context = new ApplicationDbContext();
            _bookRepository = new BookRepository(_context);
        }

        [TearDown]
        public void TearDown()
        {
            _context = null;
            _bookRepository = null;
        }

        [Test, Isolated]
        public void AddBookForAuthor_WhenCalledWithInvalidArgument_ShouldThrowException()
        {
            // Act / Assert
            Assert.Throws<ArgumentNullException>(() => _bookRepository.AddBookForAuthor(new Guid(), null));
        }

        [Test, Isolated]
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

            // Act
            _bookRepository.AddBookForAuthor(author.Id, book);

            // Assert
            var books = _context.Books.ToList();
            var authorBooks = _context.Authors.First().Books.ToList();
            Assert.That(books, Has.Count.EqualTo(1));
            Assert.That(authorBooks, Has.Count.EqualTo(1));
        }

        [Test, Isolated]
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

            // Act / Assert
            Assert.Throws<DataAlreadyExistsException>(() => _bookRepository.AddBookForAuthor(author.Id, book));
        }

        [Test, Isolated]
        public void DeleteBook_WhenCalledWithInvalidArgument_ShouldThrowException()
        {
            // Act / Assert
            Assert.Throws<ArgumentNullException>(() => _bookRepository.DeleteBook(null));
        }

        [Test, Isolated]
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

            // Act
            _bookRepository.DeleteBook(book);

            // Assert
            var result = _context.Books.ToList();
            Assert.That(result, Has.Count.EqualTo(0));
        }

        [Test, Isolated]
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
            Assert.Throws<DataNotFoundException>(() => _bookRepository.DeleteBook(book));
        }

        [Test, Isolated]
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

            // Act
            var result = _bookRepository.GetBookForAuthor(author.Id, book.Id);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<Book>(result);
        }

        [Test, Isolated]
        public void GetBookForAuthor_WhenAuthorDoesNotHaveBookInDatabase_ShouldThrowException()
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
                Books = new List<Book>(),
                DateOfBirth = new DateTimeOffset(),
                FirstName = "John",
                LastName = "Smith",
                Genre = "Adventure"
            };

            // Act / Assert
            Assert.Throws<DataNotFoundException>(() => _bookRepository.GetBookForAuthor(author.Id, book.Id));
        }

        [Test, Isolated]
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

            // Act
            var result = _bookRepository.GetBooksForAuthor(author.Id);

            // Assert
            Assert.That(result, Has.Count.EqualTo(2));
        }

        [Test, Isolated]
        public void GetBooksForAuthor_WhenAuthorHasNoBooksInDatabase_ShouldThrowException()
        {
            // Act / Assert
            Assert.Throws<DataNotFoundException>(() => _bookRepository.GetBooksForAuthor(new Guid()));
        }

        [Test, Isolated]
        public void UpdateBookForAuthor_WhenCalledWithInvalidArgument_ShouldThrowException()
        {
            // Act / Assert
            Assert.Throws<ArgumentNullException>(() => _bookRepository.UpdateBookForAuthor(new Guid(), null));
        }

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

            // Act
            _bookRepository.UpdateBookForAuthor(author.Id, book);

            // Assert
            Assert.AreEqual(newTitle, author.Books.First().Title);
            Assert.AreEqual(newDescription, author.Books.First().Description);
        }
        public void UpdateBookForAuthor_WhenCalled_ShouldDisallowIdChanges()
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

            book.Id = new Guid();

            // Act / Assert
            Assert.Throws<DataCannotChangeIdException>(() => _bookRepository.UpdateBookForAuthor(author.Id, book));
        }
    }
}
