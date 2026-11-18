namespace UsuariosAPI.Application.Interfaces
{
    using APICore.Responses;
    using UsuariosAPI.SDK.Contracts.DTOs.Contratista;

    public interface IContratistaServiceConsumer
    {
        Task<(ContratistaUsuarioDTO?, Error?)> ValidateContratista(int bdOrigenId, bool gdiAuth = true);

        Task<(IEnumerable<ContratistaUsuarioDTO>, Error?)> GetContratistasByIds(List<int> bdOrigenIds, int pageSize = 10, int pageNumber = 1, bool gdiAuth = true);
    }
}
