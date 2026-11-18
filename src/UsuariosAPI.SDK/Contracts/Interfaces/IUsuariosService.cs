namespace UsuariosAPI.SDK.Contracts.Interfaces
{
    using APICore.Interfaces.Responses;
    using APICore.Requests.DTOs;
    using APICore.Responses.DTOs;
    using UsuariosAPI.SDK.Contracts.DTOs.AdUsuario;
    using UsuariosAPI.SDK.Contracts.DTOs.CompatibilidadRol;
    using UsuariosAPI.SDK.Contracts.DTOs.Directorio;
    using UsuariosAPI.SDK.Contracts.DTOs.Usuario;

    public interface IUsuariosService
    {
        Task<IResponse<RolDTO>> GetRolById(int id, bool gdiAuth = true);

        Task<IResponse<List<RolDTO>>> GetRoles(bool gdiAuth = true);

        Task<IResponse<UsuarioRolesPermisosDTO>> GetRolesByIdRed(string idRed);

        Task<IResponse> AgregarRolesAUsuarios(AgregarRolesUsuarioDTO input);

        Task<IResponse<UsuarioDTO>> GetUsuarioById(int id, bool gdiAuth = true);

        Task<IResponse<UsuarioDTO>> GetUsuarioByIdRed(string idRed, bool gdiAuth = true);

        Task<IPaginatedResponse<UsuarioAcotadoDTO>> GetUsuariosByIds(GetByIdsListDTO dto, bool gdiAuth = true);

        Task<IPaginatedResponse<UsuarioAcotadoDTO>> GetUsuariosByRolesId(GetByIdsListDTO dto, bool gdiAuth = true);

        Task<IPaginatedResponse<UsuariosDTO>> GetUsuarios(GetPaginatedUsuariosDTO dto, bool gdiAuth = true);

        Task<IResponse> EliminarRolUsuariosBatch(UpdateUsuarioRolesBatchDTO input);

        /// <summary>
        /// Igual que <see cref="EliminarRolUsuariosBatch"/>, pero autentica con el token de aplicación de Azure AD
        /// y llama a la ruta azureAD. Usar desde procesos sin usuario logueado.
        /// </summary>
        Task<IResponse> EliminarRolUsuariosBatchAzureAD(UpdateUsuarioRolesBatchDTO input);

        Task<IPaginatedResponse<ReporteUsuariosRolesSRCDTO>> GetUsuariosRolesSRC(GetPaginatedReporteUsuariosRolesSRCDTO dto, bool gdiAuth = true);

        Task<IPaginatedResponse<RolDetailsDTO>> GetRolesByIds(GetByIdsListDTO dto, bool gdiAuth = true);

        Task<IResponse<List<int>>> GetPermisosByRolesIds(GetByIdsListDTO dto, bool gdiAuth = true);

        Task<IResponse<bool>> ValidarCompatibilidadRoles(ValidarCompatibilidadRolesDTO dto, bool gdiAuth = true);

        Task<IResponse<UsuarioDTO>> AsegurarUsuarioInterno(AsegurarUsuarioInternoDTO dto);

        Task<IPaginatedResponse<UsuariosDTO>> BuscarAdUsuariosExistentes(string palabraClave, bool gdiAuth = true);

        Task<IResponse<List<UsuarioDirectorioDTO>>> BuscarUsuariosDirectorio(BuscarUsuarioDirectorioDTO dto, bool gdiAuth = true);

        Task<IResponse<UsuarioDirectorioResueltoDTO>> ResolverUsuarioConRol(ResolverUsuarioConRolDTO dto, bool gdiAuth = true);

        Task<IResponse<List<int>>> GetUsuariosIds(GetUsuariosIdsDTO dto, bool gdiAuth = true);
    }
}
