using MyProject.Infrastructure;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Xml.Linq;

//todo: add logging (log4net?)
namespace MyProject.Repository
{

    public class GenericRepository<T> : IDisposable, IGenericRepository<T> where T : class
    {
        protected CustomerDataEntities _dataContext;

        protected string ConnectionString { get; set; }

        protected DbContext DataContext
        {
            get { return this._dataContext; }
        }

        /// <summary>
        /// Db conn. string as a param.
        /// </summary>
        /// <param name="connectionString"></param>
        /// <remarks></remarks>
        public GenericRepository(string connectionString)
        {
            this._dataContext = new CustomerDataEntities(connectionString);
            this.ConnectionString = connectionString;
        }

        public IEnumerable<T> GetAll(Func<T, bool> predicate = null)
        {
            var query = this.DataContext.Set<T>();
            if (predicate != null)
            {
                return query.Where(predicate).ToList();
            }
            return query.ToList();
        }

        public virtual int GetCount(Func<T, bool> predicate = null)
        {
            int count = 0;
            if (predicate != null)
            {
                count = this.DataContext.Set<T>().Count(predicate);
            }
            else
            {
                count = this.DataContext.Set<T>().Count();
            }
            return count;
        }

        public T FindBy(Func<T, bool> predicate)
        {
            return this.GetAll(predicate).FirstOrDefault();
        }

        public T Add(T entity)
        {
            var ent = this.DataContext.Set<T>().Add(entity);
            this.Commit();
            return ent;
        }

        public bool Delete(Func<T, bool> predicate) // T entity)
        {
            var entity = this.FindBy(predicate);
            if (entity != null)
            {
                this.DataContext.Set<T>().Remove(entity);
                this.Commit();
                return true;
            }
            return false;
        }

        public void Edit(T entity)
        {
            this.DataContext.Entry(entity).State = System.Data.Entity.EntityState.Modified;
            this.Commit();
        }

        public IEnumerable<T> GetTable(string query)
        {
            var qry = this.DataContext.Database.SqlQuery<T>(query);
            return qry;
        }

        /// <summary>
        /// data Context -> Submit Changes
        /// </summary>
        /// <remarks></remarks>
        public void Commit()
        {
            this.DataContext.SaveChanges();
        }

        public bool DatabaseExists()
        {
            return this.DataContext.Database.Exists();
        }

        #region "IDisposable Support"
        // To detect redundant calls
        private bool disposedValue;

        // IDisposable
        protected void Dispose(bool disposing)
        {
            if (!this.disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                    if ((this._dataContext != null))
                    {
                        this._dataContext.Dispose();
                    }
                }

                // TODO: free unmanaged resources (unmanaged objects) and override Finalize() below.
                // TODO: set large fields to null.
            }
            this.disposedValue = true;
        }

        // This code added by Visual Basic to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code.  Put cleanup code in Dispose(disposing As Boolean) above.
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion

    }

}
