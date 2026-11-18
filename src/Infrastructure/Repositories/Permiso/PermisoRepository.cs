namespace UsuariosAPI.Infrastructure.Repositories.Permiso
{
    using UsuariosAPI.Domain.Permisos;

    public sealed class PermisoRepository : BaseRepository<Permiso>, IPermisoRepository
    {
        public PermisoRepository(ApplicationDbContext dbContext)
            : base(dbContext)
        {
        }
    }
}
