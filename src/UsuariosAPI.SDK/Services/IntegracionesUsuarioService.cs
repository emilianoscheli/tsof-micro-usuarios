namespace UsuariosAPI.SDK.Services
{
    using APICore.Helpers;
    using APICore.Interfaces.Responses;
    using APICore.Interfaces.Services;
    using APICore.Requests.DTOs;
    using Microsoft.Extensions.Configuration;
    using UsuariosAPI.SDK.Contracts.DTOs.Usuario;
    using UsuariosAPI.SDK.Contracts.Interfaces;

    public class IntegracionesUsuarioService : IIntegracionesUsuarioService
    {
        public IntegracionesUsuarioService(IHttpClientService httpClientService, IConfiguration configuration, IAzureAdAuthClient azureAdAuthClient)
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

        public async Task<IPaginatedResponse<UsuarioAcotadoDTO>> GetUsuariosByIds(GetByIdsListDTO dto)
        {
            var parameters = QueryHelpers.ToQueryKeyValuePairs(dto);
            var azureAdToken = await this.AzureAdAuthClient.GetTokenAsync();
            return await this.HttpClientService.GetResourcesAsync<UsuarioAcotadoDTO>(this.BaseAddressAPI, "integraciones/batch/usuarios", parameters!, azureAdToken)!;
        }
    }
}
