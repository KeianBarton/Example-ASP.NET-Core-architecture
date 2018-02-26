using Library.Controllers.API;
using Library.Domain.Dtos;
using Library.Domain.Entities;
using Library.EntityFramework.Exceptions;
using Library.Services;
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
            _authorServiceMock = null;
        }

        [Test]
        public async Task GetAuthors_WhenCallingValidRequest_ShouldReturnOk()
        {
            // Arrange
            var author1 = new Author()
            {
                DateOfBirth = DateTimeOffset.Now,
                FirstName = "John",
                LastName = "Smith",
                Genre = "Adventure"
            };
            var author2 = new Author()
            {
                DateOfBirth = DateTimeOffset.Now,
                FirstName = "Stephen",
                LastName = "King",
                Genre = "Horror"
            };
            var authors = new List<Author>() { author1, author2 };
            _authorServiceMock.Setup(a => a.GetAuthors()).Returns(authors);

            // Act
            IActionResult actionResult = await _authorsApiController.GetAuthors();
            OkObjectResult result = actionResult as OkObjectResult; // null if cast fails
            List<Author> messages = result.Value as List<Author>; // null if cast fails

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
        public async Task GetAuthors_WithInvalidRequest_ShouldReturnBadRequest()
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
            var author = new Author()
            {
                DateOfBirth = DateTimeOffset.Now,
                FirstName = "John",
                LastName = "Smith",
                Genre = "Adventure"
            };
            _authorServiceMock.Setup(a => a.GetAuthor(It.IsAny<Guid>())).Returns(author);

            // Act
            IActionResult actionResult = await _authorsApiController.GetAuthor(new Guid());
            OkObjectResult result = actionResult as OkObjectResult; // null if cast fails
            Author message = result.Value as Author; // null if cast fails

            // Assert
            Assert.IsNotNull(actionResult);
            Assert.IsNotNull(result);
            Assert.IsNotNull(message);
            Assert.AreEqual(author, message);
        }

        [Test]
        public async Task GetAuthor_WhenNoData_ShouldReturnNotFound()
        {
            // Arrange
            _authorServiceMock.Setup(a => a.GetAuthor(It.IsAny<Guid>())).Throws<DataNotFoundException>();

            // Act
            var result = await _authorsApiController.GetAuthor(new Guid());

            // Assert
            Assert.IsInstanceOf<NotFoundObjectResult>(result);
        }

        [Test]
        public async Task GetAuthor_WithInvalidRequest_ShouldReturnBadRequest()
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
            var linkToResource = "url that links to where author can be accessed";
            var authorDto = new AuthorDto()
            {
                DateOfBirth = DateTimeOffset.Now,
                FirstName = "John",
                LastName = "Smith",
                Genre = "Adventure"
            };
            _authorServiceMock.Setup(a => a.AddAuthor(It.IsAny<Author>())).Returns(authorId);
            _urlHelperMock.Setup(m => m.Link(It.IsAny<string>(), It.IsAny<object>()))
                .Returns(linkToResource);

            // Act
            IActionResult actionResult = await _authorsApiController.AddAuthor(authorDto);
            CreatedResult result = actionResult as CreatedResult; // null if cast fails

            // Assert
            Assert.IsNotNull(actionResult);
            Assert.IsNotNull(result);
            Assert.AreEqual(linkToResource, result.Location);
            Assert.IsInstanceOf<Author>(result.Value);
        }

        [Test]
        public async Task AddAuthor_WhenCallingWithoutValidData_ShouldReturnBadRequest()
        {
            // Arrange
            _authorServiceMock.Setup(a => a.AddAuthor(It.IsAny<Author>())).Throws<Exception>();

            // Act
            var result = await _authorsApiController.AddAuthor(
                new AuthorDto(){ FirstName = "The only completed field" });

            // Assert
            Assert.IsInstanceOf<BadRequestObjectResult>(result);
        }
    }
}
