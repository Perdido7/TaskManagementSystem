using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Linq.Expressions;

namespace TaskManagementSystem.DataAccess
{
    public class UnitOfWork<T> : IDisposable, IUnitOfWork<T> where T : class
    {
        private readonly TaskManagementSystemDbContext _context;
        private readonly ILogger<UnitOfWork<T>> _logger;

        private bool disposed = false;

        public UnitOfWork(TaskManagementSystemDbContext context, ILogger<UnitOfWork<T>> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<T> GetByIdAsync(object id)
        {
            try
            {
                var entity = await _context.Set<T>().FindAsync(id);

                if (entity == null)
                {
                    _logger.LogWarning($"Entity of type {typeof(T).Name} with ID {id} not found.");
                }

                return entity;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while fetching entity of type {typeof(T).Name} with ID {id}.");
                throw;
            }
        }

        public async Task<T[]> GetManyAsync(params Expression<Func<T, bool>>[]? expressions)
        {
            var query = _context.Set<T>().AsQueryable();

            if (expressions != null && expressions.Any()) {
                foreach (var expression in expressions) { 
                query = query.Where(expression);
                }
            }

            return await query.ToArrayAsync();
        }

        public async Task InsertAsync(T entity)
        {
            await _context.Set<T>().AddAsync(entity);
        }

        public void Update(T entity)
        {
            _context.Set<T>().Update(entity);
        }

        public async Task<bool> SaveChangesAsync()
        {
            try
            {
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error ocurred while saving changes.");
                return false;
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed && disposing)
            {
                _context.Dispose();
            }

            disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
