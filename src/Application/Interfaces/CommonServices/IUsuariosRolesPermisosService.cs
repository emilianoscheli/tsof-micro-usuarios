namespace UsuariosAPI.Application.Interfaces.CommonServices
{
    using APICore.Responses.DTOs;

    public interface IUsuariosRolesPermisosService
    {
        Task<UsuarioRolesPermisosDTO?> GetRolesPermisosUsuarios(string idRed, CancellationToken cancellationToken = default);
    }
}
