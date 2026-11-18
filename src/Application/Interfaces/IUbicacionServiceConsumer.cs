namespace UsuariosAPI.Application.Interfaces
{
    using APICore.Requests.DTOs;
    using APICore.Responses;
    using MaestrosAPI.SDK.Contracts.DTOs.Ubicaciones;

    public interface IUbicacionServiceConsumer
    {
        Task<(PaisDTO?, Error?)> ValidatePais(int bdOrigenId, bool validarEstado = true, bool gdiAuth = true);

        Task<(ProvinciaDTO?, Error?)> ValidateProvincia(int bdOrigenId, bool validarEstado = true, bool gdiAuth = true);

        Task<(LocalidadDTO?, Error?)> ValidateLocalidad(int bdOrigenId, bool validarEstado = true, bool gdiAuth = true);

        Task<(CodigoPostalDTO?, Error?)> ValidateCodigoPostal(int bdOrigenId, bool gdiAuth = true);

        Task<(IEnumerable<PaisDTO>, Error?)> GetPaisesByIds(List<int> bdOrigenIds, bool gdiAuth = true);

        Task<(IEnumerable<ProvinciaDTO>, Error?)> GetProvinciasByIds(List<int> bdOrigenIds, bool gdiAuth = true);

        Task<(IEnumerable<LocalidadDTO>, Error?)> GetLocalidadesByIds(List<int> bdOrigenIds, bool gdiAuth = true);

        Task<(IEnumerable<CodigoPostalDTO>, Error?)> GetCodigosPostalesByIds(List<int> bdOrigenIds, bool gdiAuth = true);

        Task<bool> EsArgentina(int paisFkId);

        bool EsArgentina(string? nombrePais);
    }
}
