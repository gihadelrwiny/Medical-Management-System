using Clinical.Application.DTOs.Pagination;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Clinical.Application.Interfaces
{

    public interface IRepository<T> where T : class
    {
        Task<PagedResult<T>> GetPagedAsync(
         int pageNumber,
         int pageSize,
         Expression<Func<T, bool>>? filter = null,
         Func<IQueryable<T>, IQueryable<T>>? orderBy = null,
         Expression<Func<T, object>>[]? includes = null,
         CancellationToken cancellationToken = default);

        Task<IEnumerable<T>> GetAllAsync(
         Expression<Func<T, object>>[]? includes = null,
         CancellationToken cancellationToken = default);

        Task<T?> GetByIdAsync(
            int id,
            Expression<Func<T, object>>[]? includes = null,
            CancellationToken cancellationToken = default);

        Task<IEnumerable<T>> FindAsync(
            Expression<Func<T, bool>> predicate,
            Expression<Func<T, object>>[]? includes = null,
            CancellationToken cancellationToken = default);

        Task AddAsync(
            T entity,
            CancellationToken cancellationToken = default);

        void Update(T entity);

        void Delete(T entity);

        Task<bool> ExistsAsync(
            Expression<Func<T, bool>> predicate,
            CancellationToken cancellationToken = default);
    }
}