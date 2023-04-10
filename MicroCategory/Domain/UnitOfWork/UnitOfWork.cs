using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace MicroCategory.Domain.UnitOfWork
{
    public class UnitOfWork<TContext> : IUnitOfWork<TContext> where TContext : DbContext
    {
        protected readonly TContext DbContext;
        private bool _disposed = false;

        /// <summary>
        /// Initialize a new instace of the <see cref="UnitOfWork"/> class
        /// </summary>
        /// <param name="dbContext"></param>
        public UnitOfWork(TContext dbContext)
        {
            DbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        /// <summary>
        /// Gets the db context.
        /// </summary>
        /// <returns>The instance of type <typeparamref name="TContext"/>.</returns>
        public TContext _context => DbContext;

        public IDbContextTransaction BeginTransaction()
        {
            return DbContext.Database.BeginTransaction();
        }

        public async Task<IDbContextTransaction> BeginTransactionAync()
        {
            return await DbContext.Database.BeginTransactionAsync();
        }

        public async Task<int> Commit()
        {
            if (_disposed)
                throw new ObjectDisposedException(GetType().FullName);

            return await DbContext.SaveChangesAsync();
        }

        /// <summary>
        /// Dispose
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
                DbContext?.Dispose();

            _disposed = true;
        }


        /// <summary>
        /// Dispose
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void ResetContextState()
        {
            DbContext.ChangeTracker.Entries()
                              .Where(e => e.Entity != null).ToList()
                              .ForEach(e => e.State = EntityState.Detached);
        }
    }
}
