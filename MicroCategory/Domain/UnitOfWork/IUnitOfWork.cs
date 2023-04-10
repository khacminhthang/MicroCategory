using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace MicroCategory.Domain.UnitOfWork
{
    /// <summary>
    /// Interface
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IUnitOfWork<T> : IUnitOfWork where T : DbContext { }

    /// <summary>
    /// IUnitOfWork
    /// </summary>
    public interface IUnitOfWork : IDisposable
    {
        /// <summary>
        /// Saves changes to database, previously opening a transaction
        /// only when none exists. The transaction is opened with isolation
        /// level set in Unit of Work before calling this method.
        /// </summary>
        Task<int> Commit();

        /// <summary>
        /// BeginTransaction
        /// </summary>
        /// <returns></returns>
        IDbContextTransaction BeginTransaction();

        /// <summary>
        /// BeginTransactionAync
        /// </summary>
        /// <returns></returns>
        Task<IDbContextTransaction> BeginTransactionAync();

        /// <summary>
        /// Reset state database context; discard previous actions
        /// </summary>
        void ResetContextState();
    }
}
