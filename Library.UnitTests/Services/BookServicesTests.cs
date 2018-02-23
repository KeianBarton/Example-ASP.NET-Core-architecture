using Library.Domain.Entities;
using Library.EntityFramework.DatabaseContext;
using Library.EntityFramework.Exceptions;
using Library.Services;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Library.UnitTests.Services
{
    [TestFixture]
    public class BookServicesTests
    {
        private Mock<IUnitOfWork> _unitOfWork;
        private BookService _bookService;

        [SetUp]
        public void SetUp()
        {
            _unitOfWork = new Mock<IUnitOfWork>();
            _bookService = new BookService(_unitOfWork.Object);
        }

        [TearDown]
        public void TearDown()
        {
            _unitOfWork = null;
            _bookService = null;
        }

        [Test]
        public void AddBookForAuthor_WhenCalledWithInvalidArgument_ShouldThrowException()
        {
            // Act / Assert
            Assert.Throws<ArgumentNullException>(
                () => _bookService.AddBookForAuthor(new Guid(), null));
        }

        [Test]
        public void AddBookForAuthor_WhenValidRequest_ShouldCallRepositoryCreate()
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
            _unitOfWork.Setup(u => u.Authors.Read(It.IsAny<Guid>())).Returns(author);

            // Act
            _bookService.AddBookForAuthor(author.Id, book);

            // Assert
            Assert.That(author.Books, Has.Count.EqualTo(1));
        }

        [Test]
        public void AddBookForAuthor_WhenArgumentIsInvalid_ShouldThrowException()
        {
            // Act / Assert
            Assert.Throws<ArgumentNullException>(
                () => _bookService.AddBookForAuthor(new Guid(), null));
        }

        [Test]
        public void AddBookForAuthor_WhenAuthorDoesNotExist_ShouldThrowException()
        {
            // Arrange
            _unitOfWork.Setup(u => u.Authors.Read(It.IsAny<Guid>())).Returns((Author)null);

            // Act / Assert
            Assert.Throws<DataNotFoundException>(
                () => _bookService.AddBookForAuthor(new Guid(), new Book()));
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
            _unitOfWork.Setup(u => u.Authors.Read(It.IsAny<Guid>())).Returns(author);

            // Act / Assert
            Assert.Throws<DataAlreadyExistsException>(
                () => _bookService.AddBookForAuthor(author.Id, book));
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
            var author = new Author
            {
                Id = new Guid(),
                Books = new List<Book>() { book },
                DateOfBirth = new DateTimeOffset(),
                FirstName = "John",
                LastName = "Smith",
                Genre = "Adventure"
            };
            _unitOfWork.Setup(u => u.Books.Read(It.IsAny<Guid>())).Returns(book);
            _unitOfWork.Setup(u => u.Books.Delete(It.IsAny<Guid>())).Verifiable();

            // Act
            _bookService.DeleteBook(book.Id);

            // Assert
            _unitOfWork.Verify(u => u.Books.Delete(book.Id));
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
            // Arrange
            _unitOfWork.Setup(u => u.Books.Read(It.IsAny<Guid>())).Returns((Book)null);

            // Act / Assert
            Assert.Throws<DataNotFoundException>(
                () => _bookService.DeleteBook(book.Id));
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
            _unitOfWork.Setup(u => u.Authors.Read(It.IsAny<Guid>())).Returns(author);

            // Act
            var result = _bookService.GetBookForAuthor(author.Id, book.Id);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<Book>(result);
        }

        [Test]
        public void GetBookForAuthor_WhenAuthorDoesNotExist_ShouldThrowException()
        {
            // Arrange
            _unitOfWork.Setup(u => u.Authors.Read(It.IsAny<Guid>())).Returns((Author)null);

            // Act / Assert
            Assert.Throws<DataNotFoundException>(
                () => _bookService.GetBookForAuthor(new Guid(), new Guid()));
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
            _unitOfWork.Setup(u => u.Authors.Read(It.IsAny<Guid>())).Returns(author);

            // Act / Assert
            Assert.Throws<DataNotFoundException>(
                () => _bookService.GetBookForAuthor(author.Id, new Guid()));
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
            _unitOfWork.Setup(u => u.Authors.Read(It.IsAny<Guid>())).Returns(author);

            // Act
            var result = _bookService.GetBooksForAuthor(author.Id);

            // Assert
            Assert.That(result, Has.Count.EqualTo(2));
        }

        [Test]
        public void GetBooksForAuthor_WhenAuthorDoesNotExist_ShouldThrowException()
        {
            // Arrange
            _unitOfWork.Setup(u => u.Authors.Read(It.IsAny<Guid>())).Returns((Author)null);

            // Act / Assert
            Assert.Throws<DataNotFoundException>(
                () => _bookService.GetBooksForAuthor(new Guid()));
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
            _unitOfWork.Setup(u => u.Authors.Read(It.IsAny<Guid>())).Returns(author);

            // Act / Assert
            Assert.Throws<DataNotFoundException>(
                () => _bookService.GetBooksForAuthor(author.Id));
        }

        [Test]
        public void UpdateBookForAuthor_WhenCalledWithInvalidArgument_ShouldThrowException()
        {
            // Act / Assert
            Assert.Throws<ArgumentNullException>(
                () => _bookService.UpdateBookForAuthor(new Guid(), null));
        }

        [Test]
        public void UpdateBookForAuthor_WhenAuthorDoesNotExist_ShouldThrowException()
        {
            // Arrange
            _unitOfWork.Setup(u => u.Authors.Read(It.IsAny<Guid>())).Returns((Author)null);

            // Act / Assert
            Assert.Throws<DataNotFoundException>(
                () => _bookService.UpdateBookForAuthor(new Guid(), new Book()));
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
            _unitOfWork.Setup(u => u.Authors.Read(It.IsAny<Guid>())).Returns(author);

            // Act / Assert
            Assert.Throws<DataNotFoundException>(
                () => _bookService.UpdateBookForAuthor(author.Id, new Book()));
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
            _unitOfWork.Setup(u => u.Authors.Read(It.IsAny<Guid>())).Returns(author);

            var newTitle = "New test book";
            var newDescription = "New description";
            book.Title = newTitle;
            book.Description = newDescription;

            // Act
            _bookService.UpdateBookForAuthor(author.Id, book);

            // Assert
            Assert.AreEqual(newTitle, author.Books.First().Title);
            Assert.AreEqual(newDescription, author.Books.First().Description);
        }
    }
}