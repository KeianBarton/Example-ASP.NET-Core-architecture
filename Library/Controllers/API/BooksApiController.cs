using Library.Domain.Dtos;
using Library.Domain.Entities;
using Library.EntityFramework.Exceptions;
using Library.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Library.Controllers.API
{
    [Produces("application/json")]
    public class BooksApiController : Controller
    {
        private readonly IBookService _bookService;

        public BooksApiController(IBookService bookService)
        {
            _bookService = bookService;
        }

        // POST: api/books/for-author/x
        [HttpPost("{authorId}", Name = "AddBookForAuthor")]
        [Route("api/books/for-author")]
        public async Task<IActionResult> AddBookForAuthor(Guid authorId, BookDto bookDto)
        {
            try
            {
                var book = new Book()
                {
                    Title = bookDto.Title,
                    Description = bookDto.Description
                };
                await Task.Run(
                    () => _bookService.AddBookForAuthor(authorId, book));
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
            catch (DataAlreadyExistsException e)
            {
                return BadRequest(e.Message.ToString());
            }
            catch (Exception e)
            {
                return BadRequest(e.Message.ToString());
            }
        }

        // GET: api/books/x
        [HttpGet("{bookId}", Name = "BookExists")]
        [Route("api/books")]
        public async Task<IActionResult> BookExists(Guid bookId)
        {
            try
            {
                var bookExists = await Task.Run(
                    () => _bookService.BookExists(bookId));
                return Ok(bookExists);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message.ToString());
            }
        }

        // DELETE: api/books/x
        [HttpDelete("{bookId}")]
        [Route("api/books")]
        public async Task<IActionResult> DeleteBook(Guid bookId)
        {
            try
            {
                await Task.Run(
                    () => _bookService.DeleteBook(bookId));
                return Ok(bookId);
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

        // GET: api/books/x
        [HttpGet("{authorId}", Name = "GetBookForAuthor")]
        [Route("api/books")]
        public async Task<IActionResult> GetBookForAuthor(Guid authorId, Guid bookId)
        {
            try
            {
                var book = await Task.Run(
                    () => _bookService.GetBookForAuthor(authorId, bookId));
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

        // GET: api/books/for-author/x
        [HttpGet("{authorId}", Name = "GetBooksForAuthor")]
        [Route("api/books/for-author")]
        public async Task<IActionResult> GetBooksForAuthor(Guid authorId)
        {
            try
            {
                var books = await Task.Run(
                    () => _bookService.GetBooksForAuthor(authorId));
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

        // PUT: api/books/for-author/x
        [HttpPut("{authorId}", Name = "UpdateBookForAuthor")]
        [Route("api/books/for-author")]
        public async Task<IActionResult> UpdateBookForAuthor(Guid authorId, BookDto bookDto)
        {
            try
            {
                var book = new Book()
                {
                    Description = bookDto.Description,
                    Title = bookDto.Title
                };
                await Task.Run(
                    () => _bookService.UpdateBookForAuthor(authorId, book));
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
    }
}