using Library.Domain.Dtos;
using Library.EntityFramework.Exceptions;
using Library.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using System;
using System.Net;
using System.Threading.Tasks;

namespace Library.Controllers.API
{
    [Route("api/authors")]
    [Produces("application/json")]
    public class AuthorsApiController : Controller
    {
        private readonly IAuthorService _authorService;

        public AuthorsApiController(IAuthorService authorService)
        {
            _authorService = authorService;
        }

        // GET: api/authors/
        [HttpGet]
        public async Task<IActionResult> GetAuthors()
        {
            try
            {
                var authors = await Task.Run(
                    () => _authorService.GetAuthors());
                return Ok(authors);
            }
            catch (DataNotFoundException e)
            {
                return NotFound(e.Message.ToString());
            }
            catch (Exception e)
            {
                return BadRequest(e.Message.ToString());
            }
        }

        // GET: api/authors/x
        [HttpGet("{authorId}", Name = "GetAuthor")]
        public async Task<IActionResult> GetAuthor(Guid authorId)
        {
            try
            {
                var author = await Task.Run(
                    () => _authorService.GetAuthor(authorId));
                return Ok(author);
            }
            catch (DataNotFoundException e)
            {
                return NotFound(e.Message.ToString());
            }
            catch (Exception e)
            {
                return BadRequest(e.Message.ToString());
            }
        }

        // POST: api/authors
        [HttpPost("{authorId}")]
        public async Task<IActionResult> AddAuthor(AuthorDto authorDto)
        {
            try
            {
                var result = await Task.Run(
                    () => _authorService.AddAuthor(authorDto));

                // TODO
                var uri = Url.Link("GetAuthor", new { authorId = result });
                return Created(uri, authorDto);
            }
            catch (InvalidDataException e)
            {
                return BadRequest(e.Message.ToString());
            }
            catch (DataAlreadyExistsException)
            {
                return StatusCode((int)HttpStatusCode.Conflict);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message.ToString());
            }
        }

        // PUT: api/authors/x
        [HttpPut("{authorId}")]
        public async Task<IActionResult> UpdateAuthor(Guid authorId, AuthorDto authorDto)
        {
            try
            {
                await Task.Run(
                    () => _authorService.UpdateAuthor(authorId, authorDto));
                return NoContent();
            }
            catch (InvalidDataException e)
            {
                return BadRequest(e.Message.ToString());
            }
            catch (DataNotFoundException e)
            {
                return NotFound(e.Message.ToString());
            }
            catch (Exception e)
            {
                return BadRequest(e.Message.ToString());
            }
        }

        // DELETE: api/authors/x
        [HttpDelete("{authorId}")]
        public async Task<IActionResult> DeleteAuthor(Guid authorId)
        {
            try
            {
                await Task.Run(
                    () => _authorService.DeleteAuthor(authorId));
                return NoContent();
            }
            catch (DataNotFoundException e)
            {
                return NotFound(e.Message.ToString());
            }
            catch (Exception e)
            {
                return BadRequest(e.Message.ToString());
            }
        }
    }
}