﻿using Library.Domain.Dtos;
using Library.EntityFramework.Exceptions;
using Library.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Net;
using System.Threading.Tasks;

namespace Library.Controllers.API
{
    [Route("api/books")]
    [Produces("application/json")]
    public class BooksApiController : Controller
    {
        private readonly IBookService _bookService;

        public BooksApiController(IBookService bookService)
        {
            _bookService = bookService;
        }

        // POST: api/books/x
        [HttpPost("{authorId}", Name = "AddBookForAuthor")]
        public async Task<IActionResult> AddBookForAuthor(Guid authorId, BookDto bookDto)
        {
            try
            {
                var result = await Task.Run(
                    () => _bookService.AddBookForAuthor(authorId, bookDto));

                // TODO
                var url = Url.Link("GetBookForAuthor", new { bookId = result, authorId });
                return Created(url, bookDto);
            }
            catch (InvalidDataException e)
            {
                return BadRequest(e.Message.ToString());
            }
            catch (DataNotFoundException e)
            {
                return NotFound(e.Message.ToString());
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

        // DELETE: api/books/x
        [HttpDelete("{bookId}", Name = "DeleteBook")]
        public async Task<IActionResult> DeleteBook(Guid bookId)
        {
            try
            {
                await Task.Run(() =>
                    _bookService.DeleteBook(bookId));
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

        // GET: api/books/x/authors/y
        [HttpGet(Name = "GetBookForAuthor")]
        [Route("{bookId}/authors/{authorId}")]
        public async Task<IActionResult> GetBookForAuthor(Guid authorId, Guid bookId)
        {
            try
            {
                var book = await Task.Run(() =>
                    _bookService.GetBookForAuthor(authorId, bookId));
                return Ok(book);
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

        // GET: api/books/authors/x
        [HttpGet(Name = "GetBooksForAuthor")]
        [Route("")]
        [Route("authors/{authorId}")]
        public async Task<IActionResult> GetBooksForAuthor(Guid authorId)
        {
            try
            {
                var books = await Task.Run(() =>
                    _bookService.GetBooksForAuthor(authorId));
                return Ok(books);
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

        // PUT: api/books/authors/x
        [HttpPut("{authorId}", Name = "UpdateBookForAuthor")]
        [Route("")]
        [Route("authors")]
        public async Task<IActionResult> UpdateBookForAuthor(Guid authorId, Guid bookId, BookDto bookDto)
        {
            try
            {
                await Task.Run(() =>
                    _bookService.UpdateBookForAuthor(authorId, bookId, bookDto));
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
    }
}