// pluskal

using System;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Threading.Tasks;
using UnitOfWork;

namespace EF6UnitOfWork
{
    /// <summary>
    ///     Entity Framework IUnitOfWork Implementations
    /// </summary>
    public class Ef6UnitOfWork : IUnitOfWork, IDisposable
    {
        private readonly IsolationLevel _isolationLevel;
        private DbContextTransaction _transaction;

        public Ef6UnitOfWork(DbContext context, IsolationLevel isolationLevel)
        {
            this.DbContext = context;
            this._isolationLevel = isolationLevel;
        }

        #region Class Members

        public DbContext DbContext { get; }

        #endregion Class Members

        #region Interfaces Members

        public void BeginTransaction()
        {
            this._transaction = this.DbContext.Database.BeginTransaction(this._isolationLevel);
        }

        public void Commit()
        {
            this._transaction?.Commit();
            this.CleanUpTransaction();
        }

        public void Rollback()
        {
            this._transaction?.Rollback();
            this.CleanUpTransaction();
        }

        /// <exception cref="DbUpdateException">An error occurred sending updates to the database.</exception>
        /// <exception cref="DbEntityValidationException">
        ///     The save was aborted because validation of entity property values failed.
        /// </exception>
        /// <exception cref="DbUpdateConcurrencyException">
        ///     A database command did not affect the expected number of rows. This usually indicates an optimistic
        ///     concurrency violation; that is, a row has been changed in the database since it was queried.
        /// </exception>
        /// <exception cref="NotSupportedException">
        ///     An attempt was made to use unsupported behavior such as executing multiple asynchronous commands concurrently
        ///     on the same context instance.
        /// </exception>
        /// <exception cref="ObjectDisposedException">The context or connection have been disposed.</exception>
        /// <exception cref="InvalidOperationException">
        ///     Some error occurred attempting to process entities in the context either before or after sending commands
        ///     to the database.
        /// </exception>
        public void SaveChanges()
        {
            this.DbContext.SaveChanges();
        }

        /// <exception cref="DbUpdateException">An error occurred sending updates to the database.</exception>
        /// <exception cref="DbUpdateConcurrencyException">
        ///     A database command did not affect the expected number of rows. This usually indicates an optimistic
        ///     concurrency violation; that is, a row has been changed in the database since it was queried.
        /// </exception>
        /// <exception cref="DbUpdateConcurrencyException">
        ///     A database command did not affect the expected number of rows. This usually indicates an optimistic
        ///     concurrency violation; that is, a row has been changed in the database since it was queried.
        /// </exception>
        /// <exception cref="DbUpdateConcurrencyException">
        ///     A database command did not affect the expected number of rows. This usually indicates an optimistic
        ///     concurrency violation; that is, a row has been changed in the database since it was queried.
        /// </exception>
        /// <exception cref="DbEntityValidationException">
        ///     The save was aborted because validation of entity property values failed.
        /// </exception>
        /// <exception cref="NotSupportedException">
        ///     An attempt was made to use unsupported behavior such as executing multiple asynchronous commands concurrently
        ///     on the same context instance.
        /// </exception>
        /// <exception cref="ObjectDisposedException">The context or connection have been disposed.</exception>
        /// <exception cref="InvalidOperationException">
        ///     Some error occurred attempting to process entities in the context either before or after sending commands
        ///     to the database.
        /// </exception>
        public Task SaveChangesAsync()
        {
            return this.DbContext.SaveChangesAsync();
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion Interfaces Members

        #region Private Members

        private void CleanUpTransaction()
        {
            this._transaction = null;
        }

        private void Dispose(Boolean disposing)
        {
            if (disposing)
            {
                this._transaction?.Dispose();
                this.DbContext.Dispose();
            }

            // get rid of unmanaged resources
        }

        ~Ef6UnitOfWork()
        {
            this.Dispose(false);
        }

        #endregion Private Members
    }
}