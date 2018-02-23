using System;
using System.Collections.Generic;

namespace Library.EntityFramework.Repositories
{
    public interface IRepository<T> where T : class
    {
        void Create(T item);

        IEnumerable<T> ReadAll();

        T Read(Guid id);

        void Update(T item);

        void Delete(Guid id);
    }
}