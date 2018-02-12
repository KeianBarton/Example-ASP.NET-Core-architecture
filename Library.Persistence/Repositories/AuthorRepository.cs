using Library.Domain.Entities;
using Library.Domain.Persistence;
using Library.Domain.Persistence.Repositories;
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
            _context.Authors.Add(author);
        }

        public bool AuthorExists(Guid authorId)
        {
            return _context.Authors.Any(a => a.Id == authorId);
        }

        public void DeleteAuthor(Author author)
        {
            _context.Authors.Remove(author);
        }

        public Author GetAuthor(Guid authorId)
        {
            return _context.Authors.SingleOrDefault(a => a.Id == authorId);
        }

        public IEnumerable<Author> GetAuthors()
        {
            return _context.Authors.OrderBy(a => a.FirstName).ThenBy(a => a.LastName);
        }

        public IEnumerable<Author> GetAuthors(IEnumerable<Guid> authorIds)
        {
            return _context.Authors.Where(a => authorIds.Contains(a.Id))
                .OrderBy(a => a.FirstName)
                .OrderBy(a => a.LastName)
                .ToList();
        }

        public void UpdateAuthor(Author author)
        {
            throw new NotImplementedException();
        }
    }
}
