namespace UsuariosAPI.Application.Interfaces
{
    using APICore.Responses;
    using UsuariosAPI.SDK.Contracts.DTOs.Sociedad;

    public interface ISociedadServiceConsumer
    {
        Task<(SociedadUsuarioDTO?, Error?)> ValidateSociedad(int bdOrigenId);

        Task<(IEnumerable<SociedadUsuarioDTO>, Error?)> GetSociedadesByIds(List<int> bdOrigenIds, int pageSize = 10, int pageNumber = 1, bool gdiAuth = true);
    }
}
