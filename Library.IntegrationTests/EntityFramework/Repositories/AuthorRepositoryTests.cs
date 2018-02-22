using Library.Domain.Entities;
using Library.EntityFramework.DatabaseContext;
using Library.EntityFramework.Repositories;
using Microsoft.EntityFrameworkCore.Storage;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Library.IntegrationTests.EntityFramework.Repositories
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
        public void Create_WhenCalled_ShouldAddAuthorToDatabase()
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
            _authorRepository.Create(author);
            _unitOfWork.Complete();

            // Assert
            var result = _context.Authors.ToList();
            Assert.That(result, Has.Count.EqualTo(numberOfAuthorsBeforeChanges + 1));
        }

        [Test]
        public void Delete_WhenCalled_ShouldDeleteAuthorFromDatabase()
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
            _authorRepository.Delete(author.Id);
            _unitOfWork.Complete();

            // Assert
            var result = _context.Authors.ToList();
            Assert.That(result, Has.Count.EqualTo(numberOfAuthorsBeforeChanges));
        }

        [Test]
        public void Read_WhenAuthorNotInDatabase_ShouldReturnNull()
        {
            // Arrange
            _context.Authors.RemoveRange(_context.Authors);
            _context.SaveChanges();

            // Act
            var result = _authorRepository.Read(Guid.NewGuid());

            // Assert
            Assert.IsNull(result);
        }

        [Test]
        public void Read_WhenAuthorExists_ShouldGetAuthorAndBooksFromDatabase()
        {
            // Arrange
            var books = new List<Book>() {
                new Book() { Id = new Guid(), Title= "Foo" , Description = "Foo" }
            };
            var author = new Author
            {
                Id = new Guid(),
                Books = books,
                DateOfBirth = new DateTimeOffset(),
                FirstName = "John",
                LastName = "Smith",
                Genre = "Adventure"
            };
            _context.Authors.Add(author);
            _context.SaveChanges();

            // Act
            var result = _authorRepository.Read(author.Id);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Books);
        }

        [Test]
        public void ReadAll_WhenNoAuthorsInDatabase_ShouldReturnEmptyList()
        {
            // Arrange
            var authors = _context.Authors;
            _context.Authors.RemoveRange(authors);
            _context.SaveChanges();

            // Act
            var result = _authorRepository.ReadAll();

            // Assert
            Assert.IsEmpty(result);
        }

        [Test]
        public void ReadAll_WhenAuthorsExist_ShouldGetAuthorAndBooksFromDatabase()
        {
            // Arrange
            var books = new List<Book>() {
                new Book() { Id = new Guid(), Title= "Foo" , Description = "Foo" }
            };
            var author1 = new Author
            {
                Id = new Guid(),
                Books = books,
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
            var result = _authorRepository.ReadAll().ToList();

            // Assert
            Assert.That(result, Has.Count.EqualTo(numberOfAuthorsBeforeChanges + 2));
            Assert.IsNotNull(result.Single(a => a.Id == author1.Id).Books);
        }

        [Test]
        public void Update_WhenCalled_ShouldUpdateAuthorInDatabase()
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
            _authorRepository.Update(author);
            _unitOfWork.Complete();

            // Assert
            Assert.AreEqual(books, author.Books);
            Assert.AreEqual(dateOfBirth, author.DateOfBirth);
            Assert.AreEqual(firstName, author.FirstName);
            Assert.AreEqual(lastName, author.LastName);
            Assert.AreEqual(genre, author.Genre);
        }
    }
}