namespace UsuariosAPI.Infrastructure.Repositories.DelegacionTemporal
{
    using UsuariosAPI.Domain.DelegacionTemporal;
    using UsuariosAPI.Domain.DelegacionTemporal.Repository;

    public class RolTemporalRepository : BaseRepository<RolTemporal>, IRolTemporalRepository
    {
        public RolTemporalRepository(ApplicationDbContext dbContext)
            : base(dbContext)
        {
        }
    }
}
