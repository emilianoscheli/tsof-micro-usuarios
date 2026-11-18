namespace UsuariosAPI.Infrastructure.Repositories.CompatibilidadRol
{
    using APICore.Abstractions;
    using UsuariosAPI.Domain.CompatibilidadRol;

    public sealed class CompatibilidadRolRepository : BaseRepository<CompatibilidadRol>, ICompatibilidadRolRepository
    {
        public CompatibilidadRolRepository(ApplicationDbContext dbContext)
            : base(dbContext)
        {
        }

        public bool EsCompatible(List<int> rolesActualesIds, int rolDestinoId)
        {
            if (rolesActualesIds.Count == 0)
            {
                return true;
            }

            return rolesActualesIds.All(rolActualId =>
                this.DbContext.CompatibilidadRol.Any(c =>
                    c.RolOrigenId == rolActualId && c.RolDestinoId == rolDestinoId));
        }

        public List<int> GetRolesIncompatibles(List<int> rolesActualesIds, List<int> rolesDestinoIds)
        {
            if (rolesActualesIds.Count == 0)
            {
                return [];
            }

            return rolesDestinoIds
                .Where(rolDestinoId => !this.EsCompatible(rolesActualesIds, rolDestinoId))
                .ToList();
        }
    }
}
