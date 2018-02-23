using Library.Domain.Entities;
using Library.EntityFramework.DatabaseContext;
using Library.EntityFramework.Exceptions;
using Library.Services;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace Library.UnitTests.Services
{
    [TestFixture]
    public class AuthorServicesTests
    {
        private Mock<IUnitOfWork> _unitOfWork;
        private AuthorService _authorService;

        [SetUp]
        public void SetUp()
        {
            _unitOfWork = new Mock<IUnitOfWork>();
            _authorService = new AuthorService(_unitOfWork.Object);
        }

        [TearDown]
        public void TearDown()
        {
            _unitOfWork = null;
            _authorService = null;
        }

        [Test]
        public void AddAuthor_WhenCalledWithInvalidArgument_ShouldThrowException()
        {
            // Act / Assert
            Assert.Throws<ArgumentNullException>(
                () => _authorService.AddAuthor(null));
        }

        [Test]
        public void AddAuthor_WhenValidRequest_ShouldCallRepositoryCreate()
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
            _unitOfWork.Setup(u => u.Authors.Create(It.IsAny<Author>())).Verifiable();

            // Act
            _authorService.AddAuthor(author);

            // Assert
            _unitOfWork.Verify(u => u.Authors.Create(author));
        }

        [Test]
        public void AddAuthor_WhenAuthorAlreadyInDatabase_ShouldThrowException()
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
            Assert.Throws<DataAlreadyExistsException>(
                () => _authorService.AddAuthor(author));
        }

        [Test]
        public void AuthorExists_WhenCalledWithExistingAuthor_ShouldReturnTrue()
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

            // Act
            var result = _authorService.AuthorExists(author.Id);

            // Assert
            Assert.True(result);
        }

        [Test]
        public void AuthorExists_WhenCalledWithoutExistingAuthor_ShouldReturnFalse()
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
            _unitOfWork.Setup(u => u.Authors.Read(It.IsAny<Guid>())).Returns((Author)null);

            // Act
            var result = _authorService.AuthorExists(author.Id);

            // Assert
            Assert.False(result);
        }

        [Test]
        public void DeleteAuthor_WhenAuthorExists_ShouldCallRepositoryDelete()
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
            _unitOfWork.Setup(u => u.Authors.Delete(It.IsAny<Guid>())).Verifiable();

            // Act
            _authorService.DeleteAuthor(author.Id);

            // Assert
            _unitOfWork.Verify(u => u.Authors.Delete(author.Id));
        }

        [Test]
        public void DeleteAuthor_WhenAuthorNotInDatabase_ShouldThrowException()
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
            _unitOfWork.Setup(u => u.Authors.Read(It.IsAny<Guid>())).Returns((Author)null);

            // Act / Assert
            Assert.Throws<DataNotFoundException>(
                () => _authorService.DeleteAuthor(author.Id));
        }

        [Test]
        public void GetAuthor_WhenAuthorExists_ShouldGetAuthorFromDatabase()
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

            // Act
            var result = _authorService.GetAuthor(author.Id);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<Author>(result);
        }

        [Test]
        public void GetAuthor_WhenAuthorNotInDatabase_ShouldThrowException()
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
            _unitOfWork.Setup(u => u.Authors.Read(It.IsAny<Guid>())).Returns((Author)null);

            // Act / Assert
            Assert.Throws<DataNotFoundException>(
                () => _authorService.GetAuthor(author.Id));
        }

        [Test]
        public void GetAuthors_WhenAuthorsExist_ShouldBeTakenFromDatabase()
        {
            // Arrange
            var author1 = new Author
            {
                Id = new Guid(),
                Books = new List<Book>(),
                DateOfBirth = new DateTimeOffset(),
                FirstName = "John",
                LastName = "Smith",
                Genre = "Adventure"
            };
            var author2 = new Author
            {
                Id = new Guid(),
                Books = new List<Book>(),
                DateOfBirth = new DateTimeOffset(),
                FirstName = "Jacob",
                LastName = "Smith",
                Genre = "Horror"
            };
            var authors = new List<Author>() { author1, author2 };
            _unitOfWork.Setup(u => u.Authors.ReadAll()).Returns(authors);

            // Act
            var result = _authorService.GetAuthors();

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<IEnumerable<Author>>(result);
        }

        [Test]
        public void GetAuthors_WhenAuthorNotInDatabase_ShouldThrowException()
        {
            // Arrange
            _unitOfWork.Setup(u => u.Authors.ReadAll()).Returns(new List<Author>());

            // Act / Assert
            Assert.Throws<DataNotFoundException>(
                () => _authorService.GetAuthors());
        }

        [Test]
        public void UpdateAuthor_WhenCalledWithInvalidArgument_ShouldThrowException()
        {
            // Act / Assert
            Assert.Throws<ArgumentNullException>(
                () => _authorService.UpdateAuthor(new Guid(), null));
        }

        [Test]
        public void UpdateAuthor_WhenCalled_ShouldUpdateAuthorInDatabase()
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

            var books = new List<Book>() {
                new Book() { Id = new Guid(), Title= "Foo" , Description = "Foo" }
            };
            var dateOfBirth = new DateTimeOffset(DateTime.Now);
            var firstName = "Harry";
            var lastName = "Harry";
            var genre = "Horror";

            author.Books = books;
            author.DateOfBirth = dateOfBirth;
            author.FirstName = firstName;
            author.LastName = lastName;
            author.Genre = genre;

            // Act
            _authorService.UpdateAuthor(author.Id, author);

            // Assert
            Assert.AreEqual(books, author.Books);
            Assert.AreEqual(dateOfBirth, author.DateOfBirth);
            Assert.AreEqual(firstName, author.FirstName);
            Assert.AreEqual(lastName, author.LastName);
            Assert.AreEqual(genre, author.Genre);
        }

        [Test]
        public void UpdateAuthor_WhenAuthorDoesNotExist_ShouldThrowException()
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
            _unitOfWork.Setup(u => u.Authors.Read(It.IsAny<Guid>())).Returns((Author)null);

            // Act / Assert
            Assert.Throws<DataNotFoundException>(
                () => _authorService.UpdateAuthor(new Guid(), author));
        }
    }
}