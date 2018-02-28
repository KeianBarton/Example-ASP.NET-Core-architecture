using System;
using System.Collections.Generic;

namespace Library.EntityFramework.Repositories
{
    public interface IRepository<T> where T : class
    {
        Guid Create(T item);

        IEnumerable<T> ReadAll();

        T Read(Guid id);

        IEnumerable<T> Read(Func<T, bool> predicate);

        void Update(T item);

        void Delete(Guid id);
    }
}