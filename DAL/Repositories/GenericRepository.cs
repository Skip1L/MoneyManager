using System.Linq.Expressions;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Services.RepositoryInterfaces;

namespace DAL.Repositories
{
    public class GenericRepository<TEntity>(ApplicationContext repositoryContext)
        : IGenericRepository<TEntity> where TEntity : class, IBaseEntity
    {
        protected readonly ApplicationContext _repositoryContext = repositoryContext;

        public async Task CreateAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            await _repositoryContext.Set<TEntity>().AddAsync(entity, cancellationToken);
        }

        public void Delete(TEntity entity)
        {
            if (_repositoryContext.Entry(entity).State == EntityState.Detached)
            {
                _repositoryContext.Set<TEntity>().Attach(entity);
            }

            _repositoryContext.Set<TEntity>().Remove(entity);
        }

        public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            TEntity entityToDelete = await _repositoryContext.Set<TEntity>().FindAsync(id, cancellationToken);
            Delete(entityToDelete);
        }

        public async Task<TEntity> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> filter = null, CancellationToken cancellationToken = default)
        {
            var set = _repositoryContext.Set<TEntity>();

            if (filter != null)
            {
                return await set.AsNoTracking().FirstOrDefaultAsync(filter, cancellationToken);
            }

            return await set.AsNoTracking().FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<List<TEntity>> GetByFilterAsync(Expression<Func<TEntity, bool>> filter = null, CancellationToken cancellationToken = default)
        {
            IQueryable<TEntity> query = _repositoryContext.Set<TEntity>();

            if (filter != null)
            {
                query = query.Where(filter);
            }

            return await query.AsNoTracking().ToListAsync(cancellationToken);
        }

        public async Task<List<TEntity>> GetPagedAsync(int pageSize, int pageNumber, Expression<Func<TEntity, bool>> filter = null, CancellationToken cancellationToken = default)
        {
            IQueryable<TEntity> query = _repositoryContext.Set<TEntity>();

            if (filter != null)
            {
                query = query.Where(filter);
            }

            return await query
                .Skip(pageNumber * pageSize)
                .Take(pageSize)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await _repositoryContext.SaveChangesAsync(cancellationToken);
        }

        public void Update(TEntity entity)
        {
            _repositoryContext.Attach(entity);
            _repositoryContext.Entry(entity).State = EntityState.Modified;
        }
    }
}
