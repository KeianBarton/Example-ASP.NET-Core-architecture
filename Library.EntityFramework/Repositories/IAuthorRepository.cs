using Library.Domain.Entities;
using System;
using System.Collections.Generic;

namespace Library.EntityFramework.Repositories
{
    public interface IAuthorRepository
    {
        void Create(Author author);
        IEnumerable<Author> ReadAll();
        Author Read(Guid authorId);
        void Update(Author author);
        void Delete(Guid authorId);
    }
}
