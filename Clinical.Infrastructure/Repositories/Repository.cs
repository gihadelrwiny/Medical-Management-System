using Clinical.Application.Interfaces;
using Clinical.Domain.Common.Interfaces;
using Clinical.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace Clinical.Infrastructure.Repositories
{
    public class Repository<T> : IRepository<T> where T : class, IEntity
    {
        protected readonly ClinicalDbContext _context;
        protected readonly DbSet<T> _dbSet;

        public Repository(ClinicalDbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        /// <summary>
        /// Read-only list query. Results are NOT change-tracked.
        /// </summary>
        public async Task<IEnumerable<T>> GetAllAsync(
            Expression<Func<T, object>>[]? includes = null,
            CancellationToken cancellationToken = default)
        {
            IQueryable<T> query = _dbSet;

            if (includes is not null)
            {
                foreach (var include in includes)
                {
                    query = query.Include(include);
                }
            }

            return await query
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Fetch by id. Result IS change-tracked by design, so callers can
        /// mutate the returned entity and call Update() (or just modify it
        /// and rely on SaveChangesAsync) without a second round trip.
        /// If you only need to read/display the entity, prefer FindAsync
        /// or GetAllAsync, which are no-tracking.
        /// </summary>
        public async Task<T?> GetByIdAsync(
            int id,
            Expression<Func<T, object>>[]? includes = null,
            CancellationToken cancellationToken = default)
        {
            IQueryable<T> query = _dbSet;

            if (includes is not null)
            {
                foreach (var include in includes)
                {
                    query = query.Include(include);
                }
            }

            return await query.FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
        }

        /// <summary>
        /// Read-only filtered query. Results are NOT change-tracked.
        /// </summary>
        public async Task<IEnumerable<T>> FindAsync(
            Expression<Func<T, bool>> predicate,
            Expression<Func<T, object>>[]? includes = null,
            CancellationToken cancellationToken = default)
        {
            IQueryable<T> query = _dbSet;

            if (includes is not null)
            {
                foreach (var include in includes)
                {
                    query = query.Include(include);
                }
            }

            return await query
                .Where(predicate)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }

        public async Task AddAsync(T entity, CancellationToken cancellationToken = default)
        {
            await _dbSet.AddAsync(entity, cancellationToken);
        }

        /// <summary>
        /// Marks the entire entity as Modified. If the entity was loaded
        /// via a no-tracking query (GetAllAsync/FindAsync), calling this
        /// will overwrite every column in the database with whatever is
        /// currently on the in-memory object, not just the fields that
        /// actually changed. Only call this with entities you intend to
        /// fully overwrite, or with entities loaded via GetByIdAsync.
        /// </summary>
        public void Update(T entity)
        {
            _dbSet.Update(entity);
        }

        public void Delete(T entity)
        {
            _dbSet.Remove(entity);
        }

        public async Task<bool> ExistsAsync(
            Expression<Func<T, bool>> predicate,
            CancellationToken cancellationToken = default)
        {
            return await _dbSet.AnyAsync(predicate, cancellationToken);
        }
    }
}