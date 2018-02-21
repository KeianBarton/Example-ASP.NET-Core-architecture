using Library.Domain.Entities;
using Library.Domain.Persistence;
using Library.Domain.Persistence.Repositories;
using Library.Persistence.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Library.Persistence.Repositories
{
    public class AuthorRepository : IAuthorRepository
    {
        private readonly IApplicationDbContext _context;

        public AuthorRepository(IApplicationDbContext context)
        {
            _context = context;
        }

        public void AddAuthor(Author author)
        {
            if (author == null)
                throw new ArgumentNullException();
            if (_context.Authors.Any(a =>
                a.FirstName == author.FirstName &&
                a.LastName == author.LastName &&
                a.DateOfBirth == author.DateOfBirth))
                throw new DataAlreadyExistsException();
            _context.Authors.Add(author);
        }

        public bool AuthorExists(Guid authorId)
        {
            return _context.Authors.Any(a => a.Id == authorId);
        }

        public void DeleteAuthor(Guid authorId)
        {
            if (!_context.Authors.Any(a => a.Id == authorId))
                throw new DataNotFoundException();
            var author = _context.Authors.Single(a => a.Id == authorId);
            _context.Authors.Remove(author);
        }

        public Author GetAuthor(Guid authorId)
        {
            if (!_context.Authors.Any(a => a.Id == authorId))
                throw new DataNotFoundException();
            return _context.Authors.Single(a => a.Id == authorId);
        }

        public IEnumerable<Author> GetAuthors()
        {
            if (!_context.Authors.Any())
                throw new DataNotFoundException();
            return _context.Authors.OrderBy(a => a.FirstName).ThenBy(a => a.LastName);
        }

        public IEnumerable<Author> GetAuthors(IEnumerable<Guid> authorIds)
        {
            if (authorIds == null)
                throw new ArgumentNullException();
            return _context.Authors.Where(a => authorIds.Contains(a.Id))
                .OrderBy(a => a.FirstName)
                .OrderBy(a => a.LastName)
                .ToList();
        }

        public void UpdateAuthor(Author author)
        {
            if (author == null)
                throw new ArgumentNullException();
            var authorDb = _context.Authors.SingleOrDefault(a =>
                a.Id == author.Id ||
                (a.FirstName == author.FirstName
                && a.LastName == author.LastName
                && a.DateOfBirth == author.DateOfBirth));
            if (authorDb == null)
                throw new DataNotFoundException();
            if (author.Id != authorDb.Id)
                throw new DataCannotChangeIdException();
            authorDb.Modify(author.FirstName, author.LastName,
                author.DateOfBirth, author.Genre, author.Books);
        }
    }
}
