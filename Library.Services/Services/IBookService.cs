using Library.Domain.Dtos;
using System;
using System.Collections.Generic;

namespace Library.Services
{
    public interface IBookService
    {
        Guid AddBookForAuthor(Guid authorId, BookDto bookDto);

        bool BookExists(Guid bookId);

        bool BookExists(BookDto bookDto);

        void DeleteBook(Guid bookId);

        BookDto GetBookForAuthor(Guid authorId, Guid bookId);

        IEnumerable<BookDto> GetBooksForAuthor(Guid authorId);

        void UpdateBookForAuthor(Guid authorId, Guid bookId, BookDto bookDto);
    }
}