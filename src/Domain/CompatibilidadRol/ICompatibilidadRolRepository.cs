namespace UsuariosAPI.Domain.CompatibilidadRol
{
    using UsuariosAPI.Domain.Abstractions;

    public interface ICompatibilidadRolRepository : IBaseRepository<CompatibilidadRol>
    {
        bool EsCompatible(List<int> rolesActualesIds, int rolDestinoId);

        List<int> GetRolesIncompatibles(List<int> rolesActualesIds, List<int> rolesDestinoIds);
    }
}
