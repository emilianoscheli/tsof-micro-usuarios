namespace UsuariosAPI.Infrastructure.Repositories.UsuarioSociedad
{
    using UsuariosAPI.Domain.UsuarioSociedad;

    public sealed class UsuarioSociedadRepository : BaseRepository<UsuarioSociedad>, IUsuarioSociedadRepository
    {
        public UsuarioSociedadRepository(ApplicationDbContext dbContext)
            : base(dbContext)
        {
        }
    }
}
