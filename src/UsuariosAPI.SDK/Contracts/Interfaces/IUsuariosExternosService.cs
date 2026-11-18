namespace UsuariosAPI.SDK.Contracts.Interfaces
{
    using APICore.Interfaces.Responses;
    using UsuariosAPI.SDK.Contracts.DTOs.Usuario;

    public interface IUsuariosExternosService
    {
        Task<IPaginatedResponse<UsuariosExternosDTO>> GetUsuariosExternos(GetPaginatedUsuariosExternosDTO dto, bool gdiAuth = true);

        Task<IResponse<UsuarioExternoDTO>> GetUsuarioExternoById(int id, bool gdiAuth = true);

        Task<IResponse<List<int>>> GetUsuariosExternosIds(GetUsuariosExternosIdsDTO dto, bool gdiAuth = true);
    }
}
