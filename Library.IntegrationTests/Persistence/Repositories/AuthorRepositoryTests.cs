using GigHub.IntegrationTests;
using Library.Domain.Entities;
using Library.Persistence;
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
            _authorRepository.AddAuthor(author);

            // Act
            var result = _context.Authors.ToList();

            // Assert
            Assert.That(result, Has.Count.EqualTo(1));
        }

        [Test, Isolated]
        public void DeleteAuthor_WhenCalled_ShouldDeleteAuthorFromDatabase()
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
            _authorRepository.DeleteAuthor(author);
            var result = _context.Authors.ToList();

            // Assert
            Assert.That(result, Has.Count.EqualTo(0));
        }
    }
}
