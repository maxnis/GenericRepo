using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace MyProject.Infrastructure
{
    public interface IGenericRepository<T> where T : class
    {
        IEnumerable<T> GetAll(Func<T, bool> predicate = null);
        int GetCount(Func<T, bool> predicate = null);
        T FindBy(Func<T, bool> predicate);
        T Add(T entity);
        bool Delete(Func<T, bool> predicate);
        void Edit(T entity);
        void Commit();
        bool DatabaseExists();
        void Dispose();
    }
}
