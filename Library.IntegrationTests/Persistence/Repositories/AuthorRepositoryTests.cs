using Library.Domain.Entities;
using Library.Persistence;
using Library.Persistence.Exceptions;
using Library.Persistence.Repositories;
using Microsoft.EntityFrameworkCore.Storage;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Library.IntegrationTests.Persistence.Repositories
{
    [TestFixture]
    public class AuthorRepositoryTests
    {
        private ApplicationDbContext _context;
        private AuthorRepository _authorRepository;
        private UnitOfWork _unitOfWork;
        private IDbContextTransaction _transaction;

        [SetUp]
        public void SetUp()
        {
            var factory = new ApplicationDbContextFactory();
            _context = factory.CreateDbContext(null);
            _authorRepository = new AuthorRepository(_context);
            _unitOfWork = new UnitOfWork(_context);
            _transaction = _context.Database.BeginTransaction();
        }

        [TearDown]
        public void TearDown()
        {
            _transaction.Rollback();
            _transaction = null;
            _context = null;
            _authorRepository = null;
        }

        [Test]
        public void AddAuthor_WhenCalledWithInvalidArgument_ShouldThrowException()
        {
            // Act / Assert
            Assert.Throws<ArgumentNullException>(
                () => _authorRepository.AddAuthor(null));
        }

        [Test]
        public void AddAuthor_WhenCalled_ShouldAddAuthorToDatabase()
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
            var numberOfAuthorsBeforeChanges = _context.Authors.Count();

            // Act
            _authorRepository.AddAuthor(author);
            _unitOfWork.Complete();

            // Assert
            var result = _context.Authors.ToList();
            Assert.That(result, Has.Count.EqualTo(numberOfAuthorsBeforeChanges + 1));
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
            _context.Authors.Add(author);
            _context.SaveChanges();

            // Act / Assert
            Assert.Throws<DataAlreadyExistsException>(
                () => _authorRepository.AddAuthor(author));
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
            _context.Authors.Add(author);
            _context.SaveChanges();

            // Act
            var result = _authorRepository.AuthorExists(author.Id);

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

            // Act
            var result = _authorRepository.AuthorExists(author.Id);

            // Assert
            Assert.False(result);
        }

        [Test]
        public void DeleteAuthor_WhenAuthorExists_ShouldDeleteAuthorFromDatabase()
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
            var numberOfAuthorsBeforeChanges = _context.Authors.Count();
            _context.Authors.Add(author);
            _context.SaveChanges();

            // Act
            _authorRepository.DeleteAuthor(author.Id);
            _unitOfWork.Complete();

            // Assert
            var result = _context.Authors.ToList();
            Assert.That(result, Has.Count.EqualTo(numberOfAuthorsBeforeChanges));
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

            // Act / Assert
            Assert.Throws<DataNotFoundException>(
                () => _authorRepository.DeleteAuthor(author.Id));
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
            _context.Authors.Add(author);
            _context.SaveChanges();

            // Act
            var result = _authorRepository.GetAuthor(author.Id);

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

            // Act / Assert
            Assert.Throws<DataNotFoundException>(
                () => _authorRepository.GetAuthor(author.Id));
        }

        [Test]
        public void GetAuthors_WhenCalledWithInvalidArgument_ShouldThrowException()
        {
            // Act / Assert
            Assert.Throws<ArgumentNullException>(
                () => _authorRepository.GetAuthors(null));
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
            var numberOfAuthorsBeforeChanges = _context.Authors.Count();
            _context.Authors.Add(author1);
            _context.Authors.Add(author2);
            _context.SaveChanges();

            // Act
            var result = _authorRepository.GetAuthors().ToList();

            // Assert
            Assert.That(result, Has.Count.EqualTo(numberOfAuthorsBeforeChanges + 2));
        }

        [Test]
        public void GetAuthors_WhenAuthorExistsAndSelectingId_ShouldBeTakenFromDatabase()
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
            _context.Authors.Add(author1);
            _context.Authors.Add(author2);
            _context.SaveChanges();

            // Act
            var result = _authorRepository.GetAuthors(new List<Guid>() { author1.Id });

            // Assert
            Assert.That(result, Has.Count.EqualTo(1));
        }

        [Test]
        public void GetAuthors_WhenAuthorNotInDatabase_ShouldThrowException()
        {
            // Arrange
            _context.Authors.RemoveRange(_context.Authors);
            _context.SaveChanges();

            // Act / Assert
            Assert.Throws<DataNotFoundException>(
                () => _authorRepository.GetAuthors());
        }

        [Test]
        public void UpdateAuthor_WhenCalledWithInvalidArgument_ShouldThrowException()
        {
            // Act / Assert
            Assert.Throws<ArgumentNullException>(
                () => _authorRepository.UpdateAuthor(null));
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
            _context.Authors.Add(author);

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

            _context.SaveChanges();

            // Act
            _authorRepository.UpdateAuthor(author);
            _unitOfWork.Complete();

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

            // Act / Assert
            Assert.Throws<DataNotFoundException>(
                () => _authorRepository.UpdateAuthor(author));
        }

        [Test]
        public void UpdateAuthor_WhenCalled_ShouldDisallowIdChanges()
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
            var authorWithNewId = new Author
            {
                Id = Guid.NewGuid(),
                Books = author.Books,
                DateOfBirth = author.DateOfBirth,
                FirstName = author.FirstName,
                LastName = author.LastName,
                Genre = author.Genre
            };
            _context.Authors.Add(author);
            _context.SaveChanges();

            // Act / Assert
            Assert.Throws<DataCannotChangeIdException>(
                () => _authorRepository.UpdateAuthor(authorWithNewId));
        }
    }
}