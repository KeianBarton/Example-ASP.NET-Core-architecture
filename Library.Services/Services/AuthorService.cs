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

        public Guid AddAuthor(Author author)
        {
            if (author == null)
                throw new ArgumentNullException();

            if (AuthorExists(author.Id))
                throw new DataAlreadyExistsException("Author already exists");

            _unitOfWork.Authors.Create(author);
            _unitOfWork.Complete();
            return author.Id;
        }

        public bool AuthorExists(Guid authorId)
        {
            return _unitOfWork.Authors.Read(authorId) != null;
        }

        public void DeleteAuthor(Guid authorId)
        {
            if (!AuthorExists(authorId))
                throw new DataNotFoundException("Author not found");

            _unitOfWork.Authors.Delete(authorId);
            _unitOfWork.Complete();
        }

        public Author GetAuthor(Guid authorId)
        {
            var author = _unitOfWork.Authors.Read(authorId);

            if (author == null)
                throw new DataNotFoundException("Author not found");

            return author;
        }

        public IEnumerable<Author> GetAuthors()
        {
            var authors = _unitOfWork.Authors.ReadAll();

            if (!authors.Any())
                throw new DataNotFoundException();

            return authors.OrderBy(a => a.FirstName).ThenBy(a => a.LastName);
        }

        public void UpdateAuthor(Guid id, Author author)
        {
            if (author == null)
                throw new ArgumentNullException();

            var authorDb = _unitOfWork.Authors.Read(id);

            if (authorDb == null)
                throw new DataNotFoundException("Author not found");

            authorDb.Modify(
                author.FirstName, author.LastName, author.DateOfBirth, author.Genre);
            _unitOfWork.Complete();
        }
    }
}