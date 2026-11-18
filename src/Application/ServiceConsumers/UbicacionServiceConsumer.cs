namespace UsuariosAPI.Application.ServiceConsumers
{
    using System.Threading.Tasks;
    using APICore.Requests.DTOs;
    using APICore.Responses;
    using MaestrosAPI.SDK.Contracts.DTOs.Ubicaciones;
    using MaestrosAPI.SDK.Contracts.Interfaces;
    using UsuariosAPI.Application.Interfaces;
    using UsuariosAPI.Domain.Abstractions.Errors;

    public class UbicacionServiceConsumer : IUbicacionServiceConsumer
    {
        public UbicacionServiceConsumer(IUbicacionService ubicacionService)
        {
            this.UbicacionService = ubicacionService;
        }

        private IUbicacionService UbicacionService { get; }

        public async Task<(PaisDTO?, Error?)> ValidatePais(int bdOrigenId, bool validarEstado = true, bool gdiAuth = true)
        {
            var paisResponse = await this.UbicacionService.GetPaisById(bdOrigenId, gdiAuth);

            if (paisResponse?.Data is null || !paisResponse.IsSuccess || paisResponse.Data is null)
            {
                return (null, UsuariosError.EntidadNoEncontrada("Pais"));
            }

            if (validarEstado && paisResponse.Data.Estado.HasValue && !paisResponse.Data.Estado.Value)
            {
                return (null, UsuariosError.EntidadDesactivadaNoSePuedeUtilizar("Pais"));
            }

            return (paisResponse.Data, null);
        }

        public async Task<(ProvinciaDTO?, Error?)> ValidateProvincia(int bdOrigenId, bool validarEstado = true, bool gdiAuth = true)
        {
            var provinciaResponse = await this.UbicacionService.GetProvinciaById(bdOrigenId, gdiAuth);

            if (provinciaResponse?.Data is null || !provinciaResponse.IsSuccess || provinciaResponse.Data is null)
            {
                return (null, UsuariosError.EntidadNoEncontrada("Provincia"));
            }

            if (validarEstado && provinciaResponse.Data.Estado.HasValue && !provinciaResponse.Data.Estado.Value)
            {
                return (null, UsuariosError.EntidadDesactivadaNoSePuedeUtilizar("Provincia"));
            }

            return (provinciaResponse.Data, null);
        }

        public async Task<(LocalidadDTO?, Error?)> ValidateLocalidad(int bdOrigenId, bool validarEstado = true, bool gdiAuth = true)
        {
            var response = await this.UbicacionService.GetLocalidadById(bdOrigenId, gdiAuth);

            if (response?.Data is null || !response.IsSuccess || response.Data is null)
            {
                return (null, UsuariosError.EntidadNoEncontrada("Localidad"));
            }

            if (validarEstado && response.Data.Estado.HasValue && !response.Data.Estado.Value)
            {
                return (null, UsuariosError.EntidadDesactivadaNoSePuedeUtilizar("Localidad"));
            }

            return (response.Data, null);
        }

        public async Task<(CodigoPostalDTO?, Error?)> ValidateCodigoPostal(int bdOrigenId, bool gdiAuth = true)
        {
            var response = await this.UbicacionService.GetCodigoPostalById(bdOrigenId, gdiAuth);

            if (response?.Data is null || !response.IsSuccess || response.Data is null)
            {
                return (null, UsuariosError.EntidadNoEncontrada("Codigo Postal"));
            }

            return (response.Data, null);
        }

        public async Task<bool> EsArgentina(int paisFkId)
        {
            var (pais, error) = await this.ValidatePais(paisFkId, false);

            return error == null && (pais?.EsArgentina ?? false);
        }

        public bool EsArgentina(string? nombrePais)
        {
            return "argentina".Equals(nombrePais, StringComparison.InvariantCultureIgnoreCase);
        }

        public async Task<(IEnumerable<ProvinciaDTO>, Error?)> GetProvinciasByIds(List<int> bdOrigenIds, bool gdiAuth = true)
        {
            if (bdOrigenIds.Count == 0)
            {
                return ([], null);
            }

            var apiResponse = await this.UbicacionService.GetProvinciasByIds(new GetByIdsListDTO { Ids = bdOrigenIds, PageSize = bdOrigenIds.Count, PageNumber = 1 }, gdiAuth);

            if (apiResponse is null || !apiResponse.IsSuccess)
            {
                return ([], UsuariosError.EntidadNoEncontrada("Provincia"));
            }

            var response = apiResponse.Data?.Where(x => x != null).ToList() ?? [];

            return (response!, null);
        }

        public async Task<(IEnumerable<PaisDTO>, Error?)> GetPaisesByIds(List<int> bdOrigenIds, bool gdiAuth = true)
        {
            if (bdOrigenIds.Count == 0)
            {
                return ([], null);
            }

            var apiResponse = await this.UbicacionService.GetPaisesByIds(new GetByIdsListDTO { Ids = bdOrigenIds, PageSize = bdOrigenIds.Count, PageNumber = 1 }, gdiAuth);

            if (apiResponse is null || !apiResponse.IsSuccess)
            {
                return ([], UsuariosError.EntidadNoEncontrada("Pais"));
            }

            var response = apiResponse.Data?.Where(x => x != null).ToList() ?? [];

            return (response!, null);
        }

        public async Task<(IEnumerable<LocalidadDTO>, Error?)> GetLocalidadesByIds(List<int> bdOrigenIds, bool gdiAuth = true)
        {
            if (bdOrigenIds.Count == 0)
            {
                return ([], null);
            }

            var apiResponse = await this.UbicacionService.GetLocalidadesByIds(new GetByIdsListDTO { Ids = bdOrigenIds, PageSize = bdOrigenIds.Count, PageNumber = 1 }, gdiAuth);

            if (apiResponse is null || !apiResponse.IsSuccess)
            {
                return ([], UsuariosError.EntidadNoEncontrada("Localidad"));
            }

            var response = apiResponse.Data?.Where(x => x != null).ToList() ?? [];

            return (response!, null);
        }

        public async Task<(IEnumerable<CodigoPostalDTO>, Error?)> GetCodigosPostalesByIds(List<int> bdOrigenIds, bool gdiAuth = true)
        {
            if (bdOrigenIds.Count == 0)
            {
                return ([], null);
            }

            var apiResponse = await this.UbicacionService.GetCodigosPostalesByIds(new GetByIdsListDTO { Ids = bdOrigenIds, PageSize = bdOrigenIds.Count, PageNumber = 1 }, gdiAuth);

            if (apiResponse is null || !apiResponse.IsSuccess)
            {
                return ([], UsuariosError.EntidadNoEncontrada("Codigo Postal"));
            }

            var response = apiResponse.Data?.Where(x => x != null).ToList() ?? [];

            return (response!, null);
        }
    }
}
