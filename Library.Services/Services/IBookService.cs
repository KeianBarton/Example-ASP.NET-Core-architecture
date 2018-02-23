using Library.Domain.Entities;
using System;
using System.Collections.Generic;

namespace Library.Services
{
    public interface IBookService
    {
        void AddBookForAuthor(Guid authorId, Book book);

        bool BookExists(Guid bookId);

        void DeleteBook(Guid bookId);

        Book GetBookForAuthor(Guid authorId, Guid bookId);

        IEnumerable<Book> GetBooksForAuthor(Guid authorId);

        void UpdateBookForAuthor(Guid authorId, Book book);
    }
}