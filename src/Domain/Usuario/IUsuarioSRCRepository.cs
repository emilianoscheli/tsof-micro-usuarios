namespace UsuariosAPI.Domain.Usuario
{
    using UsuariosAPI.Domain.Abstractions;
    using UsuariosAPI.Domain.Usuario.DTO;

    public interface IUsuarioSRCRepository : IBaseRepository<UsuarioSRC>
    {
        Task<UsuarioSRC?> GetByIdRedWithDependenciesAsync(string idRed, CancellationToken cancellationToken = default);

        Task<UsuarioSRC?> GetByIdRedWithAllDataAsync(string idRed, CancellationToken cancellationToken = default);

        IQueryable<UsuarioSRC> GetByIdsWithAllData(List<int> ids);

        IQueryable<UsuarioSRC> GetByIdsWithRoles(List<int> ids);

        IQueryable<UsuarioSRC> GetByRolesIdWithAllData(List<int> rolesIds);

        Task<UsuarioSRC?> GetByIdWithAllDataAsync(int id, CancellationToken cancellationToken = default);

        Task<UsuarioSRC?> GetByIdWithRolesAsync(int id, CancellationToken cancellationToken = default);

        IQueryable<UsuarioSRC> GetFilteredAndOrderedUsuarios(UsuariosSRCFilterAndOrderDTO dto);

        IQueryable<UsuarioSRC> GetFilteredAndOrderedUsuariosExternos(UsuariosExternosFilterAndOrderDTO dto);

        IQueryable<UsuariosRolesSRCRowDTO> GetUsuariosRolesSRC(GetByFiltersReporteUsuariosRolesSRCDTO filtros);

        Task<List<string>> GetIdRedsExistentesAsync(List<string> idReds, CancellationToken cancellationToken = default);
    }
}
