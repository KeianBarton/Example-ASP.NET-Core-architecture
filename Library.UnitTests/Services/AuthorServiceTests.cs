using AutoMapper;
using Library.Domain.Dtos;
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
    public class AuthorServiceTests
    {
        private Mock<IMapper> _mapperMock;
        private Mock<IUnitOfWork> _unitOfWorkMock;
        private AuthorService _authorService;

        [SetUp]
        public void SetUp()
        {
            _mapperMock = new Mock<IMapper>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _authorService = new AuthorService(_unitOfWorkMock.Object, _mapperMock.Object);
        }

        [TearDown]
        public void TearDown()
        {
            _unitOfWorkMock = null;
            _authorService = null;
        }

        [Test]
        public void AddAuthor_WhenCalledWithInvalidData_ShouldThrowException()
        {
            // Arrange
            var author = new AuthorDto
            {
                FirstName = "Only completed field"
            };

            // Act / Assert
            Assert.Throws<InvalidDataException>(
                () => _authorService.AddAuthor(author));
        }

        [Test]
        public void AddAuthor_WhenValidRequest_ShouldCallRepositoryCreate()
        {
            // Arrange
            var authorDto = new AuthorDto()
            {
                FirstName = "Stephen",
                LastName = "King",
                Genre = "Horror",
                DateOfBirth = DateTimeOffset.Now
            };
            var author = new Author();
            _unitOfWorkMock.Setup(u => u.Authors.Create(It.IsAny<Author>()))
                .Verifiable();
            _mapperMock.Setup(m => m.Map<Author>(It.IsAny<AuthorDto>()))
                .Returns(author);

            // Act
            _authorService.AddAuthor(authorDto);

            // Assert
            _unitOfWorkMock.Verify(u => u.Authors.Create(author));
        }

        [Test]
        public void AddAuthor_WhenAuthorAlreadyInDatabase_ShouldThrowException()
        {
            // Arrange
            var authorDto = new AuthorDto()
            {
                FirstName = "Stephen",
                LastName = "King",
                Genre = "Horror",
                DateOfBirth = DateTimeOffset.Now
            };
            _unitOfWorkMock.Setup(u => u.Authors.Read(It.IsAny<Func<Author, bool>>()))
                .Returns(new List<Author>() { new Author() });

            // Act / Assert
            Assert.Throws<DataAlreadyExistsException>(
                () => _authorService.AddAuthor(authorDto));
        }

        [Test]
        public void AuthorExists_WhenCalledWithExistingAuthor_ShouldReturnTrue()
        {
            // Arrange
            var author = new Author();
            _unitOfWorkMock.Setup(u => u.Authors.Read(It.IsAny<Guid>())).Returns(author);

            // Act
            var result = _authorService.AuthorExists(author.Id);

            // Assert
            Assert.True(result);
        }

        [Test]
        public void AuthorExists_WhenCalledWithoutExistingAuthor_ShouldReturnFalse()
        {
            // Arrange
            var author = new Author();
            _unitOfWorkMock.Setup(u => u.Authors.Read(It.IsAny<Guid>())).Returns((Author)null);

            // Act
            var result = _authorService.AuthorExists(author.Id);

            // Assert
            Assert.False(result);
        }

        [Test]
        public void DeleteAuthor_WhenAuthorExists_ShouldCallRepositoryDelete()
        {
            // Arrange
            var author = new Author();
            _unitOfWorkMock.Setup(u => u.Authors.Read(It.IsAny<Guid>())).Returns(author);
            _unitOfWorkMock.Setup(u => u.Authors.Delete(It.IsAny<Guid>())).Verifiable();

            // Act
            _authorService.DeleteAuthor(author.Id);

            // Assert
            _unitOfWorkMock.Verify(u => u.Authors.Delete(author.Id));
        }

        [Test]
        public void DeleteAuthor_WhenAuthorNotInDatabase_ShouldThrowException()
        {
            // Arrange
            var author = new Author();
            _unitOfWorkMock.Setup(u => u.Authors.Read(It.IsAny<Guid>())).Returns((Author)null);

            // Act / Assert
            Assert.Throws<DataNotFoundException>(
                () => _authorService.DeleteAuthor(author.Id));
        }

        [Test]
        public void GetAuthor_WhenAuthorExists_ShouldGetAuthorFromDatabase()
        {
            // Arrange
            var author = new Author();
            _unitOfWorkMock.Setup(u => u.Authors.Read(It.IsAny<Guid>())).Returns(author);
            _mapperMock.Setup(m => m.Map<Author>(It.IsAny<AuthorDto>()))
                .Returns(author);

            // Act
            var result = _authorService.GetAuthor(author.Id);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<AuthorDto>(result);
        }

        [Test]
        public void GetAuthor_WhenAuthorNotInDatabase_ShouldThrowException()
        {
            // Arrange
            var author = new Author();
            _unitOfWorkMock.Setup(u => u.Authors.Read(It.IsAny<Guid>())).Returns((Author)null);

            // Act / Assert
            Assert.Throws<DataNotFoundException>(
                () => _authorService.GetAuthor(author.Id));
        }

        [Test]
        public void GetAuthors_WhenAuthorsExist_ShouldBeTakenFromDatabase()
        {
            // Arrange
            var author1 = new Author();
            var author2 = new Author();
            var authors = new List<Author>() { author1, author2 };
            _unitOfWorkMock.Setup(u => u.Authors.ReadAll()).Returns(authors);

            // Act
            var result = _authorService.GetAuthors();

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<IEnumerable<AuthorDto>>(result);
        }

        [Test]
        public void GetAuthors_WhenAuthorNotInDatabase_ShouldThrowException()
        {
            // Arrange
            _unitOfWorkMock.Setup(u => u.Authors.ReadAll()).Returns(new List<Author>());

            // Act / Assert
            Assert.Throws<DataNotFoundException>(
                () => _authorService.GetAuthors());
        }

        [Test]
        public void UpdateAuthor_WhenCalledWithInvalidData_ShouldThrowException()
        {
            var author = new AuthorDto
            {
                FirstName = "Only completed field"
            };

            // Act / Assert
            Assert.Throws<InvalidDataException>(
                () => _authorService.UpdateAuthor(new Guid(), author));
        }

        [Test]
        public void UpdateAuthor_WhenCalled_ShouldUpdateAuthorInDatabase()
        {
            // Arrange
            var authorDto = new AuthorDto
            {
                DateOfBirth = new DateTimeOffset(),
                FirstName = "John",
                LastName = "Smith",
                Genre = "Adventure"
            };
            var author = new Author
            {
                DateOfBirth = new DateTimeOffset(),
                FirstName = "John",
                LastName = "Smith",
                Genre = "Adventure"
            };
            _unitOfWorkMock.Setup(u => u.Authors.Read(It.IsAny<Guid>())).Returns(author);

            var dateOfBirth = new DateTimeOffset(DateTime.Now);
            var firstName = "Harry";
            var lastName = "Harry";
            var genre = "Horror";

            authorDto.DateOfBirth = dateOfBirth;
            authorDto.FirstName = firstName;
            authorDto.LastName = lastName;
            authorDto.Genre = genre;

            // Act
            _authorService.UpdateAuthor(author.Id, authorDto);

            // Assert
            Assert.AreEqual(dateOfBirth, author.DateOfBirth);
            Assert.AreEqual(firstName, author.FirstName);
            Assert.AreEqual(lastName, author.LastName);
            Assert.AreEqual(genre, author.Genre);
        }

        [Test]
        public void UpdateAuthor_WhenAuthorDoesNotExist_ShouldThrowException()
        {
            // Arrange
            var author = new AuthorDto
            {
                Books = new List<BookDto>(),
                DateOfBirth = new DateTimeOffset(),
                FirstName = "John",
                LastName = "Smith",
                Genre = "Adventure"
            };
            _unitOfWorkMock.Setup(u => u.Authors.Read(It.IsAny<Guid>())).Returns((Author)null);

            // Act / Assert
            Assert.Throws<DataNotFoundException>(
                () => _authorService.UpdateAuthor(new Guid(), author));
        }
    }
}