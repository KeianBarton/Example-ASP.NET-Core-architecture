using Library.Domain.Entities;
using System;
using System.Collections.Generic;

namespace Library.Services
{
    public interface IAuthorService
    {
        Guid AddAuthor(Author author);

        bool AuthorExists(Guid authorId);

        void DeleteAuthor(Guid authorId);

        Author GetAuthor(Guid authorId);

        IEnumerable<Author> GetAuthors();

        void UpdateAuthor(Guid id, Author author);
    }
}