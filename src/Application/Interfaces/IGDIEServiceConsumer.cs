namespace UsuariosAPI.Application.Interfaces
{
    using APICore.Responses;
    using IntegracionesAPI.SDK.Contracts.DTOs.GDIE;

    public interface IGDIEServiceConsumer
    {
        Task<(GDIEResponseDTO?, Error?)> RegistrarUsuarioAsync(CrearUsuarioGDIEDTO dto);

        Task<(GDIEResponseDTO?, Error?)> ModificarDatosPersonalesAsync(ModificarUsuarioGDIEDTO dto);

        Task<(GDIEResponseDTO?, Error?)> SolicitarAccesoAsync(GestionarAccesoDTO dto);

        Task<(GDIEResponseDTO?, Error?)> CancelarAccesoAsync(GestionarAccesoDTO dto);

        Task<(GDIEResponseDTO?, Error?)> ObtenerUsuarioPorEmailAsync(string email);

        Task<(GDIEResponseDTO?, Error?)> ObtenerUsuarioPorUserIdAsync(string userId);
    }
}