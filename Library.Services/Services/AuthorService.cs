using Library.Domain.Entities;
using Library.EntityFramework.DatabaseContext;
using Library.EntityFramework.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Library.Services
{
    public class AuthorService : IAuthorService
    {
        private readonly IUnitOfWork _unitOfWork;

        public AuthorService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public void AddAuthor(Author author)
        {
            if (author == null)
                throw new ArgumentNullException();

            if (AuthorExists(author.Id))
                throw new DataAlreadyExistsException();

            _unitOfWork.Authors.Create(author);
            _unitOfWork.Complete();
        }

        public bool AuthorExists(Guid authorId)
        {
            return _unitOfWork.Authors.Read(authorId) != null;
        }

        public void DeleteAuthor(Guid authorId)
        {
            if (!AuthorExists(authorId))
                throw new DataNotFoundException();

            _unitOfWork.Authors.Delete(authorId);
            _unitOfWork.Complete();
        }

        public Author GetAuthor(Guid authorId)
        {
            var author = _unitOfWork.Authors.Read(authorId);

            if (author == null)
                throw new DataNotFoundException();

            return author;
        }

        public IEnumerable<Author> GetAuthors()
        {
            var authors = _unitOfWork.Authors.ReadAll();

            if (!authors.Any())
                throw new DataNotFoundException();

            return authors.OrderBy(a => a.FirstName).ThenBy(a => a.LastName);
        }

        public IEnumerable<Author> GetAuthors(IEnumerable<Guid> authorIds)
        {
            if (authorIds == null)
                throw new ArgumentNullException();

            return _unitOfWork.Authors.ReadAll()
                .Where(a => authorIds.Contains(a.Id))
                .OrderBy(a => a.FirstName)
                .OrderBy(a => a.LastName)
                .ToList();
        }

        public void UpdateAuthor(Author author)
        {
            if (author == null)
                throw new ArgumentNullException();

            var authorDb = _unitOfWork.Authors.Read(author.Id);

            if (authorDb == null)
                throw new DataNotFoundException();

            authorDb.Modify(author.FirstName, author.LastName,
                author.DateOfBirth, author.Genre, author.Books);
            _unitOfWork.Complete();
        }
    }
}
