using System.Linq.Expressions;

namespace TaskManagementSystem.DataAccess
{
    public interface IUnitOfWork<T> where T : class
    {
        Task<T> GetByIdAsync(object id);
        Task<T[]> GetManyAsync(params Expression<Func<T, bool>>[]? expressions);
        Task InsertAsync(T entity);
        void Update(T entity);
        Task<bool> SaveChangesAsync();
    }
}
