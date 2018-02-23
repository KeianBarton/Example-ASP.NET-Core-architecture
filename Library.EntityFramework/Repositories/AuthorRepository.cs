using Library.Domain.Entities;
using Library.EntityFramework.DatabaseContext;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Library.EntityFramework.Repositories
{
    public class AuthorRepository : IRepository<Author>
    {
        private readonly IApplicationDbContext _context;

        public AuthorRepository(IApplicationDbContext context)
        {
            _context = context;
        }

        public void Create(Author author)
        {
            _context.Authors.Add(author);
        }

        public void Delete(Guid authorId)
        {
            var author = _context.Authors.Single(a => a.Id == authorId);
            _context.Authors.Remove(author);
        }

        public Author Read(Guid authorId)
        {
            return _context.Authors.Include(a => a.Books)
                .SingleOrDefault(a => a.Id == authorId);
        }

        public IEnumerable<Author> ReadAll()
        {
            return _context.Authors.Include(a => a.Books);
        }

        public void Update(Author author)
        {
            var authorDb = _context.Authors.Single(a => a.Id == author.Id);
            authorDb = author;
        }
    }
}