namespace UsuariosAPI.Application.Interfaces
{
    using APICore.Responses;
    using UsuariosAPI.SDK.Contracts.DTOs.EstudioAuditor;
    using UsuariosAPI.SDK.Contracts.DTOs.Sede;

    public interface IEstudioSedeServiceConsumer
    {
        Task<(EstudioAuditorUsuarioDTO?, Error?)> ValidateEstudioAuditor(int bdOrigenId, bool gdiAuth = true);

        Task<(SedeUsuarioDTO?, Error?)> ValidateSede(int bdOrigenId, bool gdiAuth = true);

        Task<(IEnumerable<EstudioAuditorUsuarioDTO>, Error?)> GetEstudiosByIds(List<int> bdOrigenIds, int pageSize = 10, int pageNumber = 1, bool gdiAuth = true);

        Task<(IEnumerable<SedeUsuarioDTO>, Error?)> GetSedesByIds(List<int> bdOrigenIds, int pageSize = 10, int pageNumber = 1, bool gdiAuth = true);
    }
}
