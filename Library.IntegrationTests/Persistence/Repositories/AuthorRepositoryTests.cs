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
    public class AuthorRepositoryTests
    {
        private AuthorRepository _authorRepository;
        private ApplicationDbContext _context;

        [SetUp]
        public void SetUp()
        {
            _context = new ApplicationDbContext();
            _authorRepository = new AuthorRepository(_context);
        }

        [TearDown]
        public void TearDown()
        {
            _context = null;
            _authorRepository = null;
        }

        [Test, Isolated]
        public void AddAuthor_WhenCalledWithInvalidArgument_ShouldThrowException()
        {
            // Act / Assert
            Assert.Throws<ArgumentNullException>(() => _authorRepository.AddAuthor(null));
        }

        [Test, Isolated]
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

            // Act
            _authorRepository.AddAuthor(author);

            // Assert
            var result = _context.Authors.ToList();
            Assert.That(result, Has.Count.EqualTo(1));
        }

        [Test, Isolated]
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

            // Act / Assert
            Assert.Throws<DataAlreadyExistsException>(() => _authorRepository.AddAuthor(author));
        }

        [Test, Isolated]
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

            // Act
            var result = _authorRepository.AuthorExists(author.Id);

            // Assert
            Assert.True(result);
        }

        [Test, Isolated]
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

        [Test, Isolated]
        public void DeleteAuthor_WhenCalledWithInvalidArgument_ShouldThrowException()
        {
            // Act / Assert
            Assert.Throws<ArgumentNullException>(() => _authorRepository.DeleteAuthor(null));
        }

        [Test, Isolated]
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
            _context.Authors.Add(author);

            // Act
            _authorRepository.DeleteAuthor(author);

            // Assert
            var result = _context.Authors.ToList();
            Assert.That(result, Has.Count.EqualTo(0));
        }

        [Test, Isolated]
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
            Assert.Throws<DataNotFoundException>(() => _authorRepository.DeleteAuthor(author));
        }

        [Test, Isolated]
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

            // Act
            var result = _authorRepository.GetAuthor(author.Id);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<Author>(result);
        }

        [Test, Isolated]
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
            Assert.Throws<DataNotFoundException>(() => _authorRepository.GetAuthor(author.Id));
        }

        [Test, Isolated]
        public void GetAuthors_WhenCalledWithInvalidArgument_ShouldThrowException()
        {
            // Act / Assert
            Assert.Throws<ArgumentNullException>(() => _authorRepository.GetAuthors(null));
        }

        [Test, Isolated]
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
            _context.Authors.Add(author1);
            _context.Authors.Add(author2);

            // Act
            var result = _authorRepository.GetAuthors();

            // Assert
            Assert.That(result, Has.Count.EqualTo(2));
        }

        [Test, Isolated]
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

            // Act
            var result = _authorRepository.GetAuthors(new List<Guid>() { author1.Id });

            // Assert
            Assert.That(result, Has.Count.EqualTo(1));
        }

        [Test, Isolated]
        public void GetAuthors_WhenAuthorNotInDatabase_ShouldThrowException()
        {
            // Act / Assert
            Assert.Throws<DataNotFoundException>(() => _authorRepository.GetAuthors());
        }

        [Test, Isolated]
        public void UpdateAuthor_WhenCalledWithInvalidArgument_ShouldThrowException()
        {
            // Act / Assert
            Assert.Throws<ArgumentNullException>(() => _authorRepository.UpdateAuthor(null));
        }

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

            // Act
            _authorRepository.UpdateAuthor(author);

            // Assert
            Assert.AreEqual(books, author.Books);
            Assert.AreEqual(dateOfBirth, author.DateOfBirth);
            Assert.AreEqual(firstName, author.FirstName);
            Assert.AreEqual(lastName, author.LastName);
            Assert.AreEqual(genre, author.Genre);
        }
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
            _context.Authors.Add(author);
            author.Id = new Guid();

            // Act / Assert
            Assert.Throws<DataCannotChangeIdException>(() => _authorRepository.UpdateAuthor(author));
        }
    }
}