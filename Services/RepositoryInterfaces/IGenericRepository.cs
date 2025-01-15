using System.Linq.Expressions;
using Domain.Entities;

namespace Services.RepositoryInterfaces
{
    public interface IGenericRepository<TEntity> where TEntity : class, IBaseEntity
    {
        Task<List<TEntity>> GetByFilterAsync(Expression<Func<TEntity, bool>> filter = null, CancellationToken cancellationToken = default);
        Task<List<TEntity>> GetPagedAsync(int pageSize, int pageNumber, Expression<Func<TEntity, bool>> filter = null, CancellationToken cancellationToken = default);
        Task<TEntity> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> filter = null, CancellationToken cancellationToken = default);
        void Update(TEntity entity);
        void Delete(TEntity entity);
        Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
        Task CreateAsync(TEntity entity, CancellationToken cancellationToken = default);
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
