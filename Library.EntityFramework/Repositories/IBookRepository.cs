using Library.Domain.Entities;
using System;
using System.Collections.Generic;

namespace Library.EntityFramework.Repositories
{
    public interface IBookRepository
    {
        void Create(Book book);
        IEnumerable<Book> ReadAll();
        Book Read(Guid bookId);
        void Update(Book book);
        void Delete(Guid bookId);
    }
}
