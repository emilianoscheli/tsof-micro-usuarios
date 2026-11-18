namespace UsuariosAPI.SDK.Services
{
    using APICore.Abstractions;
    using APICore.Helpers;
    using APICore.Interfaces.Responses;
    using APICore.Interfaces.Services;
    using UsuariosAPI.SDK.Contracts.DTOs.Usuario;
    using UsuariosAPI.SDK.Contracts.Interfaces;
    using Microsoft.Extensions.Configuration;

    public class UsuariosExternosService : IUsuariosExternosService
    {
        public UsuariosExternosService(IHttpClientService httpClientService, IConfiguration configuration, IAzureAdAuthClient azureAdAuthClient)
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

        public async Task<IPaginatedResponse<UsuariosExternosDTO>> GetUsuariosExternos(GetPaginatedUsuariosExternosDTO dto, bool gdiAuth = true)
        {
            var parameters = QueryHelpers.ToQueryKeyValuePairs(dto);

            if (!gdiAuth)
            {
                var azureAdToken = await this.AzureAdAuthClient.GetTokenAsync();
                return await this.HttpClientService.GetResourcesAsync<UsuariosExternosDTO>(this.BaseAddressAPI, "usuarios-externos/azureAD", parameters!, azureAdToken)!;
            }

            return await this.HttpClientService.GetResourcesAsync<UsuariosExternosDTO>(this.BaseAddressAPI, "usuarios-externos", parameters!)!;
        }

        public async Task<IResponse<UsuarioExternoDTO>> GetUsuarioExternoById(int id, bool gdiAuth = true)
        {
            if (!gdiAuth)
            {
                var azureAdToken = await this.AzureAdAuthClient.GetTokenAsync();
                return await this.HttpClientService.GetResourceAsync<UsuarioExternoDTO>(this.BaseAddressAPI, $"usuarios-externos/azureAD/{id}", null, azureAdToken)!;
            }

            return await this.HttpClientService.GetResourceAsync<UsuarioExternoDTO>(this.BaseAddressAPI, $"usuarios-externos/{id}")!;
        }

        public async Task<IResponse<List<int>>> GetUsuariosExternosIds(GetUsuariosExternosIdsDTO dto, bool gdiAuth = true)
        {
            if (!gdiAuth)
            {
                var azureAdToken = await this.AzureAdAuthClient.GetTokenAsync();
                return await this.HttpClientService.PostResourceAsync<GetUsuariosExternosIdsDTO, List<int>>(this.BaseAddressAPI, dto, "usuarios-externos/azureAD/ids", authToken: azureAdToken)!;
            }

            return await this.HttpClientService.PostResourceAsync<GetUsuariosExternosIdsDTO, List<int>>(this.BaseAddressAPI, dto, "usuarios-externos/ids")!;
        }
    }
}
