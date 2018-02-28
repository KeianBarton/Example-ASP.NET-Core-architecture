using Library.Controllers.API;
using Library.Domain.Dtos;
using Library.EntityFramework.Exceptions;
using Library.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Library.UnitTests.Controllers.API
{
    [TestFixture]
    public class BooksApiControllerTests
    {
        private Mock<IBookService> _bookServiceMock;
        private Mock<IUrlHelper> _urlHelperMock;
        private BooksApiController _booksApiController;

        [SetUp]
        public void SetUp()
        {
            _bookServiceMock = new Mock<IBookService>();
            _urlHelperMock = new Mock<IUrlHelper>();
            _booksApiController = new BooksApiController(_bookServiceMock.Object)
            {
                Url = _urlHelperMock.Object
            };
        }

        [TearDown]
        public void TearDown()
        {
            _booksApiController = null;
            _urlHelperMock = null;
            _bookServiceMock = null;
        }

        [Test]
        public async Task AddBookForAuthor_WhenCallingValidRequest_ShouldReturnOk()
        {
            // Arrange
            var bookId = Guid.NewGuid();

            var locationUrl = "http://location";
            _bookServiceMock.Setup(a =>
                a.AddBookForAuthor(It.IsAny<Guid>(), It.IsAny<BookDto>())).Returns(bookId);
            _urlHelperMock.Setup(m => m.Link(It.IsAny<string>(), It.IsAny<object>()))
                .Returns(locationUrl);

            // Act
            IActionResult actionResult =
                await _booksApiController.AddBookForAuthor(new Guid(), new BookDto());
            CreatedResult result = actionResult as CreatedResult; // null if cast fails

            // Assert
            Assert.IsNotNull(actionResult);
            Assert.IsNotNull(result);
            Assert.AreEqual(locationUrl, result.Location);
            Assert.IsInstanceOf<BookDto>(result.Value);
        }

        [Test]
        public async Task AddBookForAuthor_WhenDataAlreadyExists_ShouldReturnConflict()
        {
            // Arrange
            _bookServiceMock.Setup(a => a.AddBookForAuthor(It.IsAny<Guid>(), It.IsAny<BookDto>()))
                .Throws<DataAlreadyExistsException>();

            // Act
            var actionResult = await _booksApiController.AddBookForAuthor(new Guid(), new BookDto());
            StatusCodeResult result = actionResult as StatusCodeResult; // null if cast fails

            // Assert
            Assert.IsNotNull(actionResult);
            Assert.IsNotNull(result);
            Assert.AreEqual(StatusCodes.Status409Conflict, result.StatusCode);
        }

        [Test]
        public async Task AddBookForAuthor_WhenCallingWithoutValidData_ShouldReturnBadRequest()
        {
            // Arrange
            _bookServiceMock.Setup(a => a.AddBookForAuthor(It.IsAny<Guid>(), It.IsAny<BookDto>()))
                .Throws<InvalidDataException>();

            // Act
            var result = await _booksApiController.AddBookForAuthor(new Guid(), new BookDto());

            // Assert
            Assert.IsInstanceOf<BadRequestObjectResult>(result);
        }

        public async Task AddBookForAuthor_WhenCallingWithoutAuthorExisting_ShouldReturnBadRequest()
        {
            // Arrange
            _bookServiceMock.Setup(a => a.AddBookForAuthor(It.IsAny<Guid>(), It.IsAny<BookDto>()))
                .Throws<DataNotFoundException>();

            // Act
            var result = await _booksApiController.AddBookForAuthor(new Guid(), new BookDto());

            // Assert
            Assert.IsInstanceOf<NotFoundObjectResult>(result);
        }

        [Test]
        public async Task AddBookForAuthor_WithGeneralInvalidRequest_ShouldReturnBadRequest()
        {
            // Arrange
            _bookServiceMock.Setup(a => a.AddBookForAuthor(It.IsAny<Guid>(), It.IsAny<BookDto>()))
                .Throws<Exception>();

            // Act
            var result = await _booksApiController.AddBookForAuthor(new Guid(), new BookDto());

            // Assert
            Assert.IsInstanceOf<BadRequestObjectResult>(result);
        }


        [Test]
        public async Task GetBooksForAuthor_WhenCallingValidRequest_ShouldReturnOk()
        {
            // Arrange
            var book1 = new BookDto();
            var book2 = new BookDto();
            var books = new List<BookDto>() { book1, book2 };
            _bookServiceMock.Setup(a => a.GetBooksForAuthor(It.IsAny<Guid>())).Returns(books);

            // Act
            IActionResult actionResult = await _booksApiController.GetBooksForAuthor(new Guid());
            OkObjectResult result = actionResult as OkObjectResult; // null if cast fails
            List<BookDto> messages = result.Value as List<BookDto>; // null if cast fails

            // Assert
            Assert.IsNotNull(actionResult);
            Assert.IsNotNull(result);
            Assert.IsNotNull(messages);
            Assert.AreEqual(2, messages.Count);
            Assert.AreEqual(book1, messages[0]);
        }

        [Test]
        public async Task GetBooksForAuthor_WhenNoData_ShouldReturnNotFound()
        {
            // Arrange
            _bookServiceMock.Setup(a => a.GetBooksForAuthor(It.IsAny<Guid>()))
                .Throws<DataNotFoundException>();

            // Act
            var result = await _booksApiController.GetBooksForAuthor(new Guid());

            // Assert
            Assert.IsInstanceOf<NotFoundObjectResult>(result);
        }

        [Test]
        public async Task GetBooksForAuthor_WithGeneralInvalidRequest_ShouldReturnBadRequest()
        {
            // Arrange
            _bookServiceMock.Setup(a => a.GetBooksForAuthor(It.IsAny<Guid>()))
                .Throws<Exception>();

            // Act
            var result = await _booksApiController.GetBooksForAuthor(new Guid());

            // Assert
            Assert.IsInstanceOf<BadRequestObjectResult>(result);
        }

        [Test]
        public async Task GetBookForAuthor_WhenCallingValidRequest_ShouldReturnOk()
        {
            // Arrange
            var bookDto = new BookDto();
            _bookServiceMock.Setup(a =>
                a.GetBookForAuthor(It.IsAny<Guid>(), It.IsAny<Guid>())).Returns(bookDto);

            // Act
            IActionResult actionResult = await _booksApiController
                .GetBookForAuthor(new Guid(), new Guid());
            OkObjectResult result = actionResult as OkObjectResult; // null if cast fails
            var message = result.Value;

            // Assert
            Assert.IsNotNull(actionResult);
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<BookDto>(message);
            Assert.AreEqual(bookDto, message);
        }

        [Test]
        public async Task GetBookForAuthor_WhenNoData_ShouldReturnNotFound()
        {
            // Arrange
            _bookServiceMock.Setup(a => a.GetBookForAuthor(It.IsAny<Guid>(), It.IsAny<Guid>()))
                .Throws<DataNotFoundException>();

            // Act
            var result = await _booksApiController.GetBookForAuthor(new Guid(), new Guid());

            // Assert
            Assert.IsInstanceOf<NotFoundObjectResult>(result);
        }

        [Test]
        public async Task GetAuthor_WithGeneralInvalidRequest_ShouldReturnBadRequest()
        {
            // Arrange
            _bookServiceMock.Setup(a => a.GetBookForAuthor(It.IsAny<Guid>(), It.IsAny<Guid>()))
                .Throws<Exception>();

            // Act
            var result = await _booksApiController.GetBookForAuthor(new Guid(), new Guid());

            // Assert
            Assert.IsInstanceOf<BadRequestObjectResult>(result);
        }

        

        [Test]
        public async Task UpdateBookForAuthor_WhenCallingValidRequest_ShouldReturnOk()
        {
            // Arrange
            _bookServiceMock.Setup(a =>
                a.UpdateBookForAuthor(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<BookDto>()));

            // Act
            IActionResult actionResult = await _booksApiController
                .UpdateBookForAuthor(new Guid(), new Guid(), new BookDto());
            NoContentResult result = actionResult as NoContentResult; // null if cast fails

            // Assert
            Assert.IsNotNull(actionResult);
            Assert.IsNotNull(result);
        }

        [Test]
        public async Task UpdateBookForAuthor_WhenDataNotFound_ShouldReturnNotFound()
        {
            // Arrange
            _bookServiceMock.Setup(a =>
            a.UpdateBookForAuthor(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<BookDto>()))
                .Throws<DataNotFoundException>();

            // Act
            var actionResult = await _booksApiController
                .UpdateBookForAuthor(new Guid(), new Guid(), new BookDto());
            NotFoundObjectResult result = actionResult as NotFoundObjectResult; // null if cast fails

            // Assert
            Assert.IsNotNull(actionResult);
            Assert.IsNotNull(result);
        }

        [Test]
        public async Task UpdateBookForAuthor_WhenCallingWithoutValidData_ShouldReturnBadRequest()
        {
            // Arrange
            _bookServiceMock.Setup(a =>
                a.UpdateBookForAuthor(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<BookDto>()))
                .Throws<InvalidDataException>();

            // Act
            var result = await _booksApiController
                .UpdateBookForAuthor(new Guid(), new Guid(), new BookDto());

            // Assert
            Assert.IsInstanceOf<BadRequestObjectResult>(result);
        }

        [Test]
        public async Task UpdateBookForAuthor_WithGeneralInvalidRequest_ShouldReturnBadRequest()
        {
            // Arrange
            _bookServiceMock.Setup(a =>
                a.UpdateBookForAuthor(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<BookDto>()))
                .Throws<Exception>();

            // Act
            var result = await _booksApiController
                .UpdateBookForAuthor(new Guid(), new Guid(), new BookDto());

            // Assert
            Assert.IsInstanceOf<BadRequestObjectResult>(result);
        }

        [Test]
        public async Task DeleteBook_WhenCallingValidRequest_ShouldReturnOk()
        {
            // Arrange
            _bookServiceMock.Setup(a => a.DeleteBook(It.IsAny<Guid>()));

            // Act
            IActionResult actionResult = await _booksApiController
                .DeleteBook(new Guid());
            NoContentResult result = actionResult as NoContentResult; // null if cast fails

            // Assert
            Assert.IsNotNull(actionResult);
            Assert.IsNotNull(result);
        }

        [Test]
        public async Task DeleteBook_WhenDataNotFound_ShouldReturnNotFound()
        {
            // Arrange
            _bookServiceMock.Setup(a => a.DeleteBook(It.IsAny<Guid>()))
                .Throws<DataNotFoundException>();

            // Act
            var actionResult = await _booksApiController
                .DeleteBook(new Guid());
            NotFoundObjectResult result = actionResult as NotFoundObjectResult; // null if cast fails

            // Assert
            Assert.IsNotNull(actionResult);
            Assert.IsNotNull(result);
        }

        [Test]
        public async Task DeleteBook_WithGeneralInvalidRequest_ShouldReturnBadRequest()
        {
            // Arrange
            _bookServiceMock.Setup(a => a.DeleteBook(It.IsAny<Guid>()))
                .Throws<Exception>();

            // Act
            var result = await _booksApiController.DeleteBook(new Guid());

            // Assert
            Assert.IsInstanceOf<BadRequestObjectResult>(result);
        }
    }
}