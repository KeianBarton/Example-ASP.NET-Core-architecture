using Library.Domain.Entities;
using System;
using System.Collections.Generic;

namespace Library.Domain.Persistence.Repositories
{
    public interface IAuthorRepository
    {
        IEnumerable<Author> GetAuthors();
        Author GetAuthor(Guid authorId);
        IEnumerable<Author> GetAuthors(IEnumerable<Guid> authorIds);
        void AddAuthor(Author author);
        void DeleteAuthor(Guid authorId);
        void UpdateAuthor(Author author);
        bool AuthorExists(Guid authorId);
    }
}
