namespace UsuariosAPI.Infrastructure.Repositories.UsuarioRol
{
    using UsuariosAPI.Domain.UsuarioRol;

    public sealed class UsuarioRolRepository : BaseRepository<UsuarioRol>, IUsuarioRolRepository
    {
        public UsuarioRolRepository(ApplicationDbContext dbContext)
            : base(dbContext)
        {
        }
    }
}
