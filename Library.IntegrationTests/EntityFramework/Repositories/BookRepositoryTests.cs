using Library.Domain.Entities;
using Library.EntityFramework.DatabaseContext;
using Library.EntityFramework.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Library.IntegrationTests.EntityFramework.Repositories
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
        public void Create_WhenCalled_ShouldAddBookToDatabase()
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
                Author = author,
                AuthorId = author.Id,
                Id = new Guid(),
                Title = "Test Book",
                Description = "Descriptive text"
            };
            var numberOfBooksBeforeChanges = _context.Books.Count();
            _context.Authors.Add(author);
            _context.SaveChanges();

            // Act
            _bookRepository.Create(book);
            _unitOfWork.Complete();

            // Assert
            var books = _context.Books.ToList();
            var authorBooks = _context.Authors.Include(a => a.Books)
                .Single(a => a.Id == author.Id).Books.ToList();
            Assert.That(books, Has.Count.EqualTo(numberOfBooksBeforeChanges + 1));
            Assert.That(authorBooks, Has.Count.EqualTo(1));
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
            var numberOfBooksBeforeChanges = _context.Books.Count();
            _context.Authors.Add(author);
            _context.SaveChanges();

            // Act
            _bookRepository.Delete(book.Id);
            _unitOfWork.Complete();

            // Assert
            var result = _context.Books.ToList();
            Assert.That(result, Has.Count.EqualTo(numberOfBooksBeforeChanges));
        }

        [Test]
        public void Read_WhenBookNotInDatabase_ShouldReturnNull()
        {
            // Arrange
            _context.Books.RemoveRange(_context.Books);
            _context.SaveChanges();

            // Act
            var result = _bookRepository.Read(Guid.NewGuid());

            // Assert
            Assert.IsNull(result);
        }

        [Test]
        public void Read_WhenBookExists_ShouldGetBookFromDatabase()
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
            var result = _bookRepository.Read(book.Id);

            // Assert
            Assert.IsNotNull(result);
        }

        [Test]
        public void ReadAll_WhenNoBooksInDatabase_ShouldReturnEmptyList()
        {
            // Arrange
            var books = _context.Books;
            _context.Books.RemoveRange(books);
            _context.SaveChanges();

            // Act
            var result = _bookRepository.ReadAll();

            // Assert
            Assert.IsEmpty(result);
        }

        [Test]
        public void ReadAll_WhenBooksExist_ShouldGetBooksFromDatabase()
        {
            // Arrange
            var books = new List<Book>() {
                new Book() { Id = new Guid(), Title= "Foo" , Description = "Foo" },
                new Book() { Id = new Guid(), Title= "Bar" , Description = "Bar" }
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
            var numberOfBooksBeforeChanges = _context.Books.Count();
            _context.Authors.Add(author);
            _context.SaveChanges();

            // Act
            var result = _bookRepository.ReadAll().ToList();

            // Assert
            Assert.That(result, Has.Count.EqualTo(numberOfBooksBeforeChanges + 2));
        }

        [Test]
        public void Update_WhenCalled_ShouldUpdateBookInDatabase()
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
            _bookRepository.Update(book);
            _unitOfWork.Complete();

            // Assert
            Assert.AreEqual(newTitle, author.Books.First().Title);
            Assert.AreEqual(newDescription, author.Books.First().Description);
        }
    }
}