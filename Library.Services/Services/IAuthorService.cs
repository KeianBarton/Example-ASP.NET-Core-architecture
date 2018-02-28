using Library.Domain.Dtos;
using System;
using System.Collections.Generic;

namespace Library.Services
{
    public interface IAuthorService
    {
        Guid AddAuthor(AuthorDto authorDto);

        bool AuthorExists(Guid authorId);

        bool AuthorExists(AuthorDto authorDto);

        void DeleteAuthor(Guid authorId);

        AuthorDto GetAuthor(Guid authorId);

        IEnumerable<AuthorDto> GetAuthors();

        void UpdateAuthor(Guid id, AuthorDto authorDto);
    }
}