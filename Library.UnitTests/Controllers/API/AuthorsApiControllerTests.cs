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
    public class AuthorsApiControllerTests
    {
        private Mock<IAuthorService> _authorServiceMock;
        private Mock<IUrlHelper> _urlHelperMock;
        private AuthorsApiController _authorsApiController;

        [SetUp]
        public void SetUp()
        {
            _authorServiceMock = new Mock<IAuthorService>();
            _urlHelperMock = new Mock<IUrlHelper>();
            _authorsApiController = new AuthorsApiController(_authorServiceMock.Object)
            {
                Url = _urlHelperMock.Object
            };
        }

        [TearDown]
        public void TearDown()
        {
            _authorsApiController = null;
            _urlHelperMock = null;
            _authorServiceMock = null;
        }

        [Test]
        public async Task GetAuthors_WhenCallingValidRequest_ShouldReturnOk()
        {
            // Arrange
            var author1 = new AuthorDto();
            var author2 = new AuthorDto();
            var authors = new List<AuthorDto>() { author1, author2 };
            _authorServiceMock.Setup(a => a.GetAuthors()).Returns(authors);

            // Act
            IActionResult actionResult = await _authorsApiController.GetAuthors();
            OkObjectResult result = actionResult as OkObjectResult; // null if cast fails
            List<AuthorDto> messages = result.Value as List<AuthorDto>; // null if cast fails

            // Assert
            Assert.IsNotNull(actionResult);
            Assert.IsNotNull(result);
            Assert.IsNotNull(messages);
            Assert.AreEqual(2, messages.Count);
            Assert.AreEqual(author1, messages[0]);
        }

        [Test]
        public async Task GetAuthors_WhenNoData_ShouldReturnNotFound()
        {
            // Arrange
            _authorServiceMock.Setup(a => a.GetAuthors()).Throws<DataNotFoundException>();

            // Act
            var result = await _authorsApiController.GetAuthors();

            // Assert
            Assert.IsInstanceOf<NotFoundObjectResult>(result);
        }

        [Test]
        public async Task GetAuthors_WithGeneralInvalidRequest_ShouldReturnBadRequest()
        {
            // Arrange
            _authorServiceMock.Setup(a => a.GetAuthors()).Throws<Exception>();

            // Act
            var result = await _authorsApiController.GetAuthors();

            // Assert
            Assert.IsInstanceOf<BadRequestObjectResult>(result);
        }

        [Test]
        public async Task GetAuthor_WhenCallingValidRequest_ShouldReturnOk()
        {
            // Arrange
            var authorDto = new AuthorDto();
            _authorServiceMock.Setup(a => a.GetAuthor(It.IsAny<Guid>())).Returns(authorDto);

            // Act
            IActionResult actionResult = await _authorsApiController.GetAuthor(new Guid());
            OkObjectResult result = actionResult as OkObjectResult; // null if cast fails
            var message = result.Value;

            // Assert
            Assert.IsNotNull(actionResult);
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<AuthorDto>(message);
            Assert.AreEqual(authorDto, message);
        }

        [Test]
        public async Task GetAuthor_WhenNoData_ShouldReturnNotFound()
        {
            // Arrange
            _authorServiceMock.Setup(a => a.GetAuthor(It.IsAny<Guid>()))
                .Throws<DataNotFoundException>();

            // Act
            var result = await _authorsApiController.GetAuthor(new Guid());

            // Assert
            Assert.IsInstanceOf<NotFoundObjectResult>(result);
        }

        [Test]
        public async Task GetAuthor_WithGeneralInvalidRequest_ShouldReturnBadRequest()
        {
            // Arrange
            _authorServiceMock.Setup(a => a.GetAuthor(It.IsAny<Guid>())).Throws<Exception>();

            // Act
            var result = await _authorsApiController.GetAuthor(new Guid());

            // Assert
            Assert.IsInstanceOf<BadRequestObjectResult>(result);
        }

        [Test]
        public async Task AddAuthor_WhenCallingValidRequest_ShouldReturnOk()
        {
            // Arrange
            var authorId = Guid.NewGuid();

            var locationUrl = "http://location";
            _authorServiceMock.Setup(a => a.AddAuthor(It.IsAny<AuthorDto>())).Returns(authorId);
            _urlHelperMock.Setup(m => m.Link(It.IsAny<string>(), It.IsAny<object>()))
                .Returns(locationUrl);

            // Act
            IActionResult actionResult = await _authorsApiController.AddAuthor(new AuthorDto());
            CreatedResult result = actionResult as CreatedResult; // null if cast fails

            // Assert
            Assert.IsNotNull(actionResult);
            Assert.IsNotNull(result);
            Assert.AreEqual(locationUrl, result.Location);
            Assert.IsInstanceOf<AuthorDto>(result.Value);
        }

        [Test]
        public async Task AddAuthor_WhenDataAlreadyExists_ShouldReturnConflict()
        {
            // Arrange
            _authorServiceMock.Setup(a => a.AddAuthor(It.IsAny<AuthorDto>()))
                .Throws<DataAlreadyExistsException>();

            // Act
            var actionResult = await _authorsApiController.AddAuthor(new AuthorDto());
            StatusCodeResult result = actionResult as StatusCodeResult; // null if cast fails

            // Assert
            Assert.IsNotNull(actionResult);
            Assert.IsNotNull(result);
            Assert.AreEqual(StatusCodes.Status409Conflict, result.StatusCode);
        }

        [Test]
        public async Task AddAuthor_WhenCallingWithoutValidData_ShouldReturnBadRequest()
        {
            // Arrange
            _authorServiceMock.Setup(a => a.AddAuthor(It.IsAny<AuthorDto>()))
                .Throws<InvalidDataException>();

            // Act
            var result = await _authorsApiController.AddAuthor(new AuthorDto());

            // Assert
            Assert.IsInstanceOf<BadRequestObjectResult>(result);
        }

        [Test]
        public async Task AddAuthor_WithGeneralInvalidRequest_ShouldReturnBadRequest()
        {
            // Arrange
            _authorServiceMock.Setup(a => a.AddAuthor(It.IsAny<AuthorDto>()))
                .Throws<Exception>();

            // Act
            var result = await _authorsApiController.AddAuthor(new AuthorDto());

            // Assert
            Assert.IsInstanceOf<BadRequestObjectResult>(result);
        }

        [Test]
        public async Task UpdateAuthor_WhenCallingValidRequest_ShouldReturnOk()
        {
            // Arrange
            _authorServiceMock.Setup(a =>
                a.UpdateAuthor(It.IsAny<Guid>(), It.IsAny<AuthorDto>()));

            // Act
            IActionResult actionResult = await _authorsApiController
                .UpdateAuthor(new Guid(), new AuthorDto());
            NoContentResult result = actionResult as NoContentResult; // null if cast fails

            // Assert
            Assert.IsNotNull(actionResult);
            Assert.IsNotNull(result);
        }

        [Test]
        public async Task UpdateAuthor_WhenDataNotFound_ShouldReturnNotFound()
        {
            // Arrange
            _authorServiceMock.Setup(a => a.UpdateAuthor(It.IsAny<Guid>(), It.IsAny<AuthorDto>()))
                .Throws<DataNotFoundException>();

            // Act
            var actionResult = await _authorsApiController
                .UpdateAuthor(new Guid(), new AuthorDto());
            NotFoundObjectResult result = actionResult as NotFoundObjectResult; // null if cast fails

            // Assert
            Assert.IsNotNull(actionResult);
            Assert.IsNotNull(result);
        }

        [Test]
        public async Task UpdateAuthor_WhenCallingWithoutValidData_ShouldReturnBadRequest()
        {
            // Arrange
            _authorServiceMock.Setup(a => a.UpdateAuthor(It.IsAny<Guid>(), It.IsAny<AuthorDto>()))
                .Throws<InvalidDataException>();

            // Act
            var result = await _authorsApiController.UpdateAuthor(new Guid(), new AuthorDto());

            // Assert
            Assert.IsInstanceOf<BadRequestObjectResult>(result);
        }

        [Test]
        public async Task UpdateAuthor_WithGeneralInvalidRequest_ShouldReturnBadRequest()
        {
            // Arrange
            _authorServiceMock.Setup(a => a.UpdateAuthor(It.IsAny<Guid>(), It.IsAny<AuthorDto>()))
                .Throws<Exception>();

            // Act
            var result = await _authorsApiController.UpdateAuthor(new Guid(), new AuthorDto());

            // Assert
            Assert.IsInstanceOf<BadRequestObjectResult>(result);
        }

        [Test]
        public async Task DeleteAuthor_WhenCallingValidRequest_ShouldReturnOk()
        {
            // Arrange
            _authorServiceMock.Setup(a =>
                a.DeleteAuthor(It.IsAny<Guid>()));

            // Act
            IActionResult actionResult = await _authorsApiController
                .DeleteAuthor(new Guid());
            NoContentResult result = actionResult as NoContentResult; // null if cast fails

            // Assert
            Assert.IsNotNull(actionResult);
            Assert.IsNotNull(result);
        }

        [Test]
        public async Task DeleteAuthor_WhenDataNotFound_ShouldReturnNotFound()
        {
            // Arrange
            _authorServiceMock.Setup(a => a.DeleteAuthor(It.IsAny<Guid>()))
                .Throws<DataNotFoundException>();

            // Act
            var actionResult = await _authorsApiController
                .DeleteAuthor(new Guid());
            NotFoundObjectResult result = actionResult as NotFoundObjectResult; // null if cast fails

            // Assert
            Assert.IsNotNull(actionResult);
            Assert.IsNotNull(result);
        }

        [Test]
        public async Task DeleteAuthor_WithGeneralInvalidRequest_ShouldReturnBadRequest()
        {
            // Arrange
            _authorServiceMock.Setup(a => a.DeleteAuthor(It.IsAny<Guid>()))
                .Throws<Exception>();

            // Act
            var result = await _authorsApiController.DeleteAuthor(new Guid());

            // Assert
            Assert.IsInstanceOf<BadRequestObjectResult>(result);
        }
    }
}