namespace UsuariosAPI.Infrastructure.Repositories
{
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using Microsoft.EntityFrameworkCore;
    using UsuariosAPI.Domain.Abstractions;

    public abstract class BaseRepository<T>(ApplicationDbContext dbContext) : IBaseRepository<T>
        where T : Entity
    {
        protected ApplicationDbContext DbContext { get; } = dbContext;

        public async Task<T?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await this.DbContext.Set<T>().FindAsync([id], cancellationToken);
        }

        public IQueryable<T> GetByIds(List<int> ids)
        {
            return this.DbContext.Set<T>().Where(x => ids.Contains(x.Id));
        }

        public async Task AddAsync(T entity, CancellationToken cancellationToken = default)
        {
            await this.DbContext.AddAsync(entity, cancellationToken);
        }

        public async Task AddRangeAsync(IEnumerable<T> entity, CancellationToken cancellationToken = default)
        {
            await this.DbContext.AddRangeAsync(entity, cancellationToken);
        }

        public void Update(T entity)
        {
            this.DbContext.Update(entity);
        }

        public async Task<T?> GetByAsync(IQueryable<T> queryable, CancellationToken cancellationToken = default)
        {
            return await queryable.FirstOrDefaultAsync(cancellationToken);
        }

        public IQueryable<T> AsQueryable()
        {
            return this.DbContext.Set<T>().AsQueryable();
        }

        public async Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await this.DbContext.Set<T>().ToListAsync(cancellationToken);
        }

        public async Task<T?> FindOneAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
        {
            return await this.DbContext.Set<T>().FirstOrDefaultAsync(predicate, cancellationToken);
        }

        public void Remove(T entity)
        {
            this.DbContext.Remove(entity);
        }

        public void RemoveRange(IEnumerable<T> entity, CancellationToken cancellationToken = default)
        {
            this.DbContext.Set<T>().RemoveRange(entity);
        }
    }
}
