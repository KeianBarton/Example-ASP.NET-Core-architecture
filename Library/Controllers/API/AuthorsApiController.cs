using Library.Domain.Dtos;
using Library.Domain.Entities;
using Library.EntityFramework.Exceptions;
using Library.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
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
                var author = new Author()
                {
                    DateOfBirth = authorDto.DateOfBirth,
                    FirstName = authorDto.FirstName,
                    LastName = authorDto.LastName,
                    Genre = authorDto.Genre,
                    Books = new List<Book>()
                };
                await Task.Run(
                    () => _authorService.AddAuthor(author));
                return Ok();
            }
            catch (ArgumentNullException e)
            {
                return BadRequest(e.Message.ToString());
            }
            catch (DataAlreadyExistsException e)
            {
                return BadRequest(e.Message.ToString());
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
                var author = new Author()
                {
                    DateOfBirth = authorDto.DateOfBirth,
                    FirstName = authorDto.FirstName,
                    LastName = authorDto.LastName,
                    Genre = authorDto.Genre,
                    Books = new List<Book>()
                };
                await Task.Run(
                    () => _authorService.UpdateAuthor(authorId, author));
                return Ok();
            }
            catch (ArgumentNullException e)
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
                return Ok(authorId);
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