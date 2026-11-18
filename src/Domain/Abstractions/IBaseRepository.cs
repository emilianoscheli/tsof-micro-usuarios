namespace UsuariosAPI.Domain.Abstractions
{
    using System.Linq.Expressions;

    public interface IBaseRepository<T>
    {
        Task<T?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

        IQueryable<T> GetByIds(List<int> ids);

        Task<T?> FindOneAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);

        Task<T?> GetByAsync(IQueryable<T> queryable, CancellationToken cancellationToken = default);

        Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default);

        IQueryable<T> AsQueryable();

        Task AddAsync(T entity, CancellationToken cancellationToken = default);

        Task AddRangeAsync(IEnumerable<T> entity, CancellationToken cancellationToken = default);

        void Update(T entity);

        void Remove(T entity);

        /// <summary>
        /// Funcion para eliminar multiples entidades en un solo proceso.
        /// </summary>
        /// <param name="entity">Lista de entidades.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        void RemoveRange(IEnumerable<T> entity, CancellationToken cancellationToken = default);
    }
}
