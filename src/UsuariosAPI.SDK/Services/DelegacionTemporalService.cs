namespace UsuariosAPI.SDK.Services
{
    using APICore.Helpers;
    using APICore.Interfaces.Responses;
    using APICore.Interfaces.Services;
    using APICore.Responses.DTOs;
    using Microsoft.Extensions.Configuration;
    using UsuariosAPI.SDK.Contracts.DTOs.DelegacionTemporal;
    using UsuariosAPI.SDK.Contracts.Interfaces;

    public class DelegacionTemporalService : IDelegacionTemporalService
    {
        public DelegacionTemporalService(IHttpClientService httpClientService, IConfiguration configuration, IAzureAdAuthClient azureAdAuthClient)
        {
            this.HttpClientService = httpClientService;
            this.Configuration = configuration;
            this.AzureAdAuthClient = azureAdAuthClient;

            var baseAddressSection = this.Configuration.GetSection("ServiceUrls:Usuarios:BaseAddress");

            if (baseAddressSection?.Value is null)
            {
                throw new ArgumentException("La API de Usuarios no tiene un valor de BaseAddress asignado en la configuración");
            }

            this.BaseAddressAPI = baseAddressSection.Value!;
        }

        private IHttpClientService HttpClientService { get; }

        private IConfiguration Configuration { get; }

        private IAzureAdAuthClient AzureAdAuthClient { get; }

        private string BaseAddressAPI { get; }

        public async Task<IPaginatedResponse<ReporteDelegacionTemporalDTO>> GetReporteDelegacionTemporal(GetPaginatedReporteDelegacionTemporalDTO dto, bool gdiAuth = true)
        {
            var parameters = QueryHelpers.ToQueryKeyValuePairs(dto);

            if (!gdiAuth)
            {
                var azureAdToken = await this.AzureAdAuthClient.GetTokenAsync();
                return await this.HttpClientService.GetResourcesAsync<ReporteDelegacionTemporalDTO>(this.BaseAddressAPI, $"delegacion-temporal/azureAD/reportes/delegaciones-temporales-usuarios", parameters!, azureAdToken)!;
            }

            return await this.HttpClientService.GetResourcesAsync<ReporteDelegacionTemporalDTO>(this.BaseAddressAPI, "delegacion-temporal/reportes/delegaciones-temporales-usuarios", parameters!)!;
        }

        public async Task<IPaginatedResponse<DelegacionTemporalDTO>> GetDelegacionesTemporales(GetDelegacionTemporalPaginatedDTO dto)
        {
            var parameters = QueryHelpers.ToQueryKeyValuePairs(dto);

            return await this.HttpClientService.GetResourcesAsync<DelegacionTemporalDTO>(this.BaseAddressAPI, "delegacion-temporal", parameters!)!;
        }

        public async Task<IResponse<DelegacionTemporalDTO>> GetDelegacionTemporalById(int id)
        {
            return await this.HttpClientService.GetResourceAsync<DelegacionTemporalDTO>(this.BaseAddressAPI, $"delegacion-temporal/{id}")!;
        }

        public async Task<IResponse<CantidadRecursosAfectadosDTO>> GetCantidadRecursosAfectadosPorMotivoDelegacion(int id)
        {
            return await this.HttpClientService.GetResourceAsync<CantidadRecursosAfectadosDTO>(this.BaseAddressAPI, $"delegacion-temporal/recursos-asociados/motivo-delegacion/{id}")!;
        }

        public async Task<IResponse<List<ExcelCantidadRegistrosAsociadosMotivoDelegacionDTO>>> GetExcelRecursosAfectadosPorMotivoDelegacion(int id)
        {
            return await this.HttpClientService.GetResourceAsync<List<ExcelCantidadRegistrosAsociadosMotivoDelegacionDTO>>(this.BaseAddressAPI, $"delegacion-temporal/recursos-asociados/motivo-delegacion/excel/{id}")!;
        }
    }
}
