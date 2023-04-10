using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace MicroCategory.Domain.Repositories
{
    public class EfRepository<TEntity> : IEfRepository<TEntity> where TEntity : class
    {

        /// <summary>
        /// Fields
        /// </summary>
        protected readonly DbContext _dbContext;
        private DbSet<TEntity> _entities;

        /// <summary>
        /// Initialize a new instance of the <see cref=" EfRepository{TEntity}"/> class
        /// </summary>
        /// <param name="dbContext"></param>
        public EfRepository(DbContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public IQueryable<TEntity> Table => Entities;

        public IQueryable<TEntity> TableNoTracking => Entities.AsNoTracking();

        public async Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await Entities.AsNoTracking().AnyAsync(predicate);
        }

        public void Delete(TEntity entity)
        {
            if (entity == null)
                throw new ArgumentException(nameof(entity));
            try
            {
                Entities.Remove(entity);
            }
            catch (DbUpdateException exception)
            {
                throw new Exception(GetFullErrorTextAndRollbackEntityChanges(exception), exception);
            }
        }

        public void DeleteRange(IEnumerable<TEntity> entities)
        {
            if (entities == null)
                throw new ArgumentNullException(nameof(entities));
            try
            {
                Entities.RemoveRange(entities);
            }
            catch (DbUpdateException exception)
            {
                throw new Exception(GetFullErrorTextAndRollbackEntityChanges(exception), exception);
            }
        }

        public void ExecuteNonQuery(string commandText, object[] parameter = null)
        {
            var dbContext = _dbContext.Database;
            var command = dbContext.GetDbConnection().CreateCommand();
            command.CommandText = commandText;

            // open connection
            dbContext.OpenConnection();

            if (parameter != null)
            {
                foreach (var entity in parameter)
                {
                    command.Parameters.Add(entity);
                }
            }

            command.ExecuteNonQuery();
            command.Dispose();
        }

        public TEntity Insert(TEntity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            try
            {
                return Entities.Add(entity).Entity;
            }
            catch (DbUpdateException exception)
            {
                throw new Exception(GetFullErrorTextAndRollbackEntityChanges(exception), exception);
            }
        }

        public void InsertRange(IEnumerable<TEntity> entities)
        {
            if (entities == null)
            {
                throw new ArgumentNullException(nameof(entities));
            }

            try
            {
                Entities.AddRange(entities);
            }
            catch (DbUpdateException exception)
            {
                throw new Exception(GetFullErrorTextAndRollbackEntityChanges(exception), exception);
            }
        }

        public async Task<TEntity> SingleOrDefaultAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await Entities.AsNoTracking().SingleOrDefaultAsync(predicate);
        }

        public void Update(TEntity entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));
            try
            {
                _dbContext.Entry(entity).State = EntityState.Modified;
            }
            catch (DbUpdateException exception)
            {
                throw new Exception(GetFullErrorTextAndRollbackEntityChanges(exception), exception);
            }
        }

        public void UpdateRange(IEnumerable<TEntity> entities)
        {
            if (entities == null)
                throw new ArgumentNullException(nameof(entities));
            try
            {
                Entities.UpdateRange(entities);
            }
            catch (DbUpdateException exception)
            {
                throw new Exception(GetFullErrorTextAndRollbackEntityChanges(exception), exception);
            }
        }

        /// <summary>
        /// Gets an entity set
        /// </summary>
        protected virtual DbSet<TEntity> Entities
        {
            get
            {
                if (_entities == null)
                    _entities = _dbContext.Set<TEntity>();

                return _entities;
            }
        }

        /// <summary>
        /// Rollback of entity changes and return full error message
        /// </summary>
        /// <param name="exception"></param>
        /// <returns></returns>
        protected string GetFullErrorTextAndRollbackEntityChanges(DbUpdateException exception)
        {
            //rollback entity changes
            if (_dbContext is DbContext dbContext)
            {
                var entries = dbContext.ChangeTracker.Entries()
                    .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified).ToList();

                entries.ForEach(entry =>
                {
                    try
                    {
                        entry.State = EntityState.Unchanged;
                    }
                    catch (InvalidOperationException)
                    {
                        //ignored
                    }
                });
            }

            try
            {
                _dbContext.SaveChanges();
                return exception.ToString();
            }
            catch (Exception ex)
            {
                //if after the rollback of changes the context is still not saving,
                //return the full text of the exception that occurred when saving
                return ex.ToString();
            }
        }
    }
}
