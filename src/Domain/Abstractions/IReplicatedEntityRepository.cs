namespace UsuariosAPI.Domain.Abstractions
{
    public interface IReplicatedEntityRepository<T> : IBaseRepository<T>
    {
        Task<T?> GetByBdOrigenIdAsync(int id, CancellationToken cancellationToken = default);
    }
}
