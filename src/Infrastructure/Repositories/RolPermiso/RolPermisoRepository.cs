namespace UsuariosAPI.Infrastructure.Repositories.RolPermiso
{
    using UsuariosAPI.Domain.RolPermiso;

    public class RolPermisoRepository : BaseRepository<RolPermiso>, IRolPermisoRepository
    {
        public RolPermisoRepository(ApplicationDbContext dbContext)
            : base(dbContext)
        {
        }
    }
}
