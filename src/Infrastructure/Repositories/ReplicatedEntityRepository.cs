namespace UsuariosAPI.Infrastructure.Repositories
{
    using Microsoft.EntityFrameworkCore;
    using UsuariosAPI.Domain.Abstractions;

    public abstract class ReplicatedEntityRepository<T> : BaseRepository<T>, IReplicatedEntityRepository<T>
        where T : Entity, IReplicatedEntity
    {
        protected ReplicatedEntityRepository(ApplicationDbContext dbContext)
            : base(dbContext)
        {
        }

        public async Task<T?> GetByBdOrigenIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await this.DbContext.Set<T>().SingleOrDefaultAsync(x => x.BdOrigenId == id, cancellationToken);
        }
    }
}
