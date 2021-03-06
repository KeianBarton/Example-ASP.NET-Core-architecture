﻿using Library.Domain.Entities;
using Library.EntityFramework.Repositories;

namespace Library.EntityFramework.DatabaseContext
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly IApplicationDbContext _context;

        public IRepository<Author> Authors { get; private set; }
        public IRepository<Book> Books { get; private set; }

        public UnitOfWork(IApplicationDbContext context)
        {
            _context = context;
            Authors = new AuthorRepository(context);
            Books = new BookRepository(context);
        }

        public void Complete()
        {
            _context.SaveChanges();
        }
    }
}