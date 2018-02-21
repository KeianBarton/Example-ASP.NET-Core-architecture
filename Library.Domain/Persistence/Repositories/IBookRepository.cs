using Library.Domain.Entities;
using System;
using System.Collections.Generic;

namespace Library.Domain.Persistence.Repositories
{
    public interface IBookRepository
    {
        IEnumerable<Book> GetBooksForAuthor(Guid authorId);
        Book GetBookForAuthor(Guid authorId, Guid bookId);
        void AddBookForAuthor(Guid authorId, Book book);
        void UpdateBookForAuthor(Guid authorId, Book book);
        void DeleteBook(Guid bookId);
    }
}
