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
using System.Linq;

namespace Library.UnitTests.Services
{
    [TestFixture]
    public class BookServiceTests
    {
        private Mock<IMapper> _mapperMock;
        private Mock<IUnitOfWork> _unitOfWorkMock;
        private BookService _bookService;

        [SetUp]
        public void SetUp()
        {
            _mapperMock = new Mock<IMapper>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _bookService = new BookService(_unitOfWorkMock.Object, _mapperMock.Object);
        }

        [TearDown]
        public void TearDown()
        {
            _unitOfWorkMock = null;
            _bookService = null;
        }

        [Test]
        public void AddBookForAuthor_WhenValidRequest_ShouldCallRepositoryCreate()
        {
            // Arrange
            var bookDto = new BookDto() { Description = "Scary book", Title = "It" };
            var book = new Book() { Description = "Scary book", Title = "It" };
            var author = new Author();
            _mapperMock.Setup(m => m.Map<Book>(It.IsAny<BookDto>()))
                .Returns(book);
            _unitOfWorkMock.Setup(u => u.Authors.Read(It.IsAny<Guid>())).Returns(author);

            // Act
            _bookService.AddBookForAuthor(new Guid(), bookDto);

            // Assert
            Assert.That(author.Books, Has.Count.EqualTo(1));
        }

        [Test]
        public void AddBookForAuthor_WhenDataIsInvalid_ShouldThrowException()
        {
            // Arrange
            var bookDto = new BookDto
            {
                Title = "Only completed field"
            };

            // Act / Assert
            Assert.Throws<InvalidDataException>(
                () => _bookService.AddBookForAuthor(new Guid(), bookDto));
        }

        [Test]
        public void AddBookForAuthor_WhenAuthorDoesNotExist_ShouldThrowException()
        {
            // Arrange
            var bookDto = new BookDto() { Description = "Scary book", Title = "It" };
            _unitOfWorkMock.Setup(u => u.Authors.Read(It.IsAny<Guid>())).Returns((Author)null);

            // Act / Assert
            Assert.Throws<DataNotFoundException>(
                () => _bookService.AddBookForAuthor(new Guid(), bookDto));
        }

        [Test]
        public void AddBookForAuthor_WhenAuthorAlreadyHasBook_ShouldThrowException()
        {
            // Arrange
            var bookDto = new BookDto() { Description = "Scary book", Title = "It" };
            var book = new Book() { Description = "Scary book", Title = "It" };
            var author = new Author() { Books = new List<Book>() { book } };
            _mapperMock.Setup(m => m.Map<Book>(It.IsAny<BookDto>()))
                .Returns(book);
            _unitOfWorkMock.Setup(u => u.Authors.Read(It.IsAny<Guid>())).Returns(author);

            // Act / Assert
            Assert.Throws<DataAlreadyExistsException>(
                () => _bookService.AddBookForAuthor(author.Id, bookDto));
        }

        [Test]
        public void DeleteBook_WhenBookExists_ShouldDeleteBookFromDatabase()
        {
            // Arrange
            var book = new Book();
            var author = new Author() { Books = new List<Book>() { book } };
            _unitOfWorkMock.Setup(u => u.Books.Read(It.IsAny<Guid>())).Returns(book);
            _unitOfWorkMock.Setup(u => u.Books.Delete(It.IsAny<Guid>())).Verifiable();

            // Act
            _bookService.DeleteBook(book.Id);

            // Assert
            _unitOfWorkMock.Verify(u => u.Books.Delete(book.Id));
        }

        [Test]
        public void DeleteBook_WhenBookNotInDatabase_ShouldThrowException()
        {
            // Arrange
            var book = new Book();
            _unitOfWorkMock.Setup(u => u.Books.Read(It.IsAny<Guid>())).Returns((Book)null);

            // Act / Assert
            Assert.Throws<DataNotFoundException>(
                () => _bookService.DeleteBook(book.Id));
        }

        [Test]
        public void GetBookForAuthor_WhenBookExists_ShouldGetBookFromDatabase()
        {
            // Arrange
            var book = new Book();
            var author = new Author { Books = new List<Book>() { book } };
            _unitOfWorkMock.Setup(u => u.Authors.Read(It.IsAny<Guid>())).Returns(author);

            // Act
            var result = _bookService.GetBookForAuthor(author.Id, book.Id);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<BookDto>(result);
        }

        [Test]
        public void GetBookForAuthor_WhenAuthorDoesNotExist_ShouldThrowException()
        {
            // Arrange
            _unitOfWorkMock.Setup(u => u.Authors.Read(It.IsAny<Guid>())).Returns((Author)null);

            // Act / Assert
            Assert.Throws<DataNotFoundException>(
                () => _bookService.GetBookForAuthor(new Guid(), new Guid()));
        }

        [Test]
        public void GetBookForAuthor_WhenAuthorDoesNotHaveBookInDatabase_ShouldThrowException()
        {
            // Arrange
            var author = new Author();
            _unitOfWorkMock.Setup(u => u.Authors.Read(It.IsAny<Guid>())).Returns(author);

            // Act / Assert
            Assert.Throws<DataNotFoundException>(
                () => _bookService.GetBookForAuthor(author.Id, new Guid()));
        }

        [Test]
        public void GetBooksForAuthor_WhenAuthorsBooksExist_ShouldBeTakenFromDatabase()
        {
            // Arrange
            var book1 = new Book();
            var book2 = new Book();
            var author = new Author() { Books = new List<Book>() { book1, book2 } };
            _unitOfWorkMock.Setup(u => u.Authors.Read(It.IsAny<Guid>())).Returns(author);
            _mapperMock.Setup(m => m.Map<Book, BookDto>(It.IsAny<Book>()))
                .Returns(new BookDto());

            // Act
            var result = _bookService.GetBooksForAuthor(author.Id).ToList();

            // Assert
            Assert.That(result, Has.Count.EqualTo(2));
        }

        [Test]
        public void GetBooksForAuthor_WhenAuthorDoesNotExist_ShouldThrowException()
        {
            // Arrange
            _unitOfWorkMock.Setup(u => u.Authors.Read(It.IsAny<Guid>())).Returns((Author)null);

            // Act / Assert
            Assert.Throws<DataNotFoundException>(
                () => _bookService.GetBooksForAuthor(new Guid()));
        }

        [Test]
        public void GetBooksForAuthor_WhenAuthorHasNoBooksInDatabase_ShouldThrowException()
        {
            // Arrange
            var author = new Author();
            _unitOfWorkMock.Setup(u => u.Authors.Read(It.IsAny<Guid>())).Returns(author);

            // Act / Assert
            Assert.Throws<DataNotFoundException>(
                () => _bookService.GetBooksForAuthor(author.Id));
        }

        [Test]
        public void UpdateBookForAuthor_WhenCalledWithInvalidData_ShouldThrowException()
        {
            // Arrange
            var bookDto = new BookDto
            {
                Title = "Only completed field"
            };

            // Act / Assert
            Assert.Throws<InvalidDataException>(
                () => _bookService.UpdateBookForAuthor(new Guid(), new Guid(), bookDto));
        }

        [Test]
        public void UpdateBookForAuthor_WhenAuthorDoesNotExist_ShouldThrowException()
        {
            // Arrange
            var bookDto = new BookDto() { Description = "Scary book", Title = "It" };
            var book = new Book() { Description = "Scary book", Title = "It" };
            _mapperMock.Setup(m => m.Map<Book>(It.IsAny<BookDto>()))
                .Returns(book);
            _unitOfWorkMock.Setup(u => u.Authors.Read(It.IsAny<Guid>())).Returns((Author)null);

            // Act / Assert
            Assert.Throws<DataNotFoundException>(
                () => _bookService.UpdateBookForAuthor(new Guid(), new Guid(), bookDto));
        }

        [Test]
        public void UpdateBookForAuthor_WhenBookDoesNotExist_ShouldThrowException()
        {
            // Arrange
            var bookDto = new BookDto() { Description = "Scary book", Title = "It" };
            var book = new Book() { Description = "Scary book", Title = "It" };
            var author = new Author();
            _mapperMock.Setup(m => m.Map<Book>(It.IsAny<BookDto>()))
                .Returns(book);
            _unitOfWorkMock.Setup(u => u.Authors.Read(It.IsAny<Guid>())).Returns(author);

            // Act / Assert
            Assert.Throws<DataNotFoundException>(
                () => _bookService.UpdateBookForAuthor(author.Id, new Guid(), bookDto));
        }

        [Test]
        public void UpdateBookForAuthor_WhenCalled_ShouldUpdateBookForAuthorInDatabase()
        {
            // Arrange
            var book = new Book() { Description = "Scary book", Title = "It" };
            var author = new Author() { Books = new List<Book>() { book } };
            _mapperMock.Setup(m => m.Map<Book>(It.IsAny<BookDto>()))
                .Returns(book);
            _unitOfWorkMock.Setup(u => u.Authors.Read(It.IsAny<Guid>())).Returns(author);

            var newTitle = "New test book";
            var newDescription = "New description";
            var bookDto = new BookDto() { Title = newTitle, Description = newDescription };

            // Act
            _bookService.UpdateBookForAuthor(author.Id, new Guid(), bookDto);

            // Assert
            Assert.AreEqual(newTitle, author.Books.First().Title);
            Assert.AreEqual(newDescription, author.Books.First().Description);
        }
    }
}