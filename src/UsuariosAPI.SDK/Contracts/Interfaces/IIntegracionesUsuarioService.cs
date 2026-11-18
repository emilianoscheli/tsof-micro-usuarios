namespace UsuariosAPI.SDK.Contracts.Interfaces
{
    using APICore.Interfaces.Responses;
    using APICore.Requests.DTOs;
    using UsuariosAPI.SDK.Contracts.DTOs.Usuario;

    public interface IIntegracionesUsuarioService
    {
        Task<IPaginatedResponse<UsuarioAcotadoDTO>> GetUsuariosByIds(GetByIdsListDTO dto);
    }
}
