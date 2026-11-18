namespace UsuariosAPI.SDK.Services
{
    using APICore.Helpers;
    using APICore.Interfaces.Responses;
    using APICore.Interfaces.Services;
    using APICore.Requests.DTOs;
    using APICore.Responses;
    using APICore.Responses.DTOs;
    using Microsoft.Extensions.Caching.Memory;
    using Microsoft.Extensions.Configuration;
    using UsuariosAPI.SDK.Contracts.DTOs.AdUsuario;
    using UsuariosAPI.SDK.Contracts.DTOs.CompatibilidadRol;
    using UsuariosAPI.SDK.Contracts.DTOs.Directorio;
    using UsuariosAPI.SDK.Contracts.DTOs.Usuario;
    using UsuariosAPI.SDK.Contracts.Interfaces;

    public class UsuariosService : IUsuariosService
    {
        private static readonly MemoryCache UsuarioAcotadoCache = new MemoryCache(new MemoryCacheOptions());

        public UsuariosService(IHttpClientService httpClientService, IConfiguration configuration, IAzureAdAuthClient azureAdAuthClient)
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

            var cacheSection = configuration.GetSection("UsuariosCache:DurationMinutes");
            var cacheMinutes = int.TryParse(cacheSection?.Value, out var parsed) ? parsed : 5;
            this.CacheDuration = TimeSpan.FromMinutes(cacheMinutes);
        }

        private IHttpClientService HttpClientService { get; }

        private IConfiguration Configuration { get; }

        private IAzureAdAuthClient AzureAdAuthClient { get; }

        private string BaseAddressAPI { get; }

        private TimeSpan CacheDuration { get; }

        public async Task<IResponse<RolDTO>> GetRolById(int id, bool gdiAuth = true)
        {
            if (!gdiAuth)
            {
                var azureAdToken = await this.AzureAdAuthClient.GetTokenAsync();
                return await this.HttpClientService.GetResourceAsync<RolDTO>(this.BaseAddressAPI, $"usuarios/azureAD/roles/{id}", authToken: azureAdToken)!;
            }

            return await this.HttpClientService.GetResourceAsync<RolDTO>(this.BaseAddressAPI, $"usuarios/roles/{id}")!;
        }

        public async Task<IResponse<List<RolDTO>>> GetRoles(bool gdiAuth = true)
        {
            if (!gdiAuth)
            {
                var azureAdToken = await this.AzureAdAuthClient.GetTokenAsync();
                return await this.HttpClientService.GetResourceAsync<List<RolDTO>>(this.BaseAddressAPI, $"usuarios/azureAD/roles", authToken: azureAdToken)!;
            }

            return await this.HttpClientService.GetResourceAsync<List<RolDTO>>(this.BaseAddressAPI, $"usuarios/roles")!;
        }

        public async Task<IResponse<UsuarioRolesPermisosDTO>> GetRolesByIdRed(string idRed)
        {
            var parameters = QueryHelpers.ToQueryKeyValuePairs(new { IdRed = idRed });

            return await this.HttpClientService.GetResourceAsync<UsuarioRolesPermisosDTO>(this.BaseAddressAPI, $"usuarios/permisos-usuario", parameters!)!;
        }

        public async Task<IResponse<UsuarioDTO>> GetUsuarioById(int id, bool gdiAuth = true)
        {
            if (!gdiAuth)
            {
                var azureAdToken = await this.AzureAdAuthClient.GetTokenAsync();
                return await this.HttpClientService.GetResourceAsync<UsuarioDTO>(this.BaseAddressAPI, $"usuarios/azureAD/{id}", authToken: azureAdToken)!;
            }

            return await this.HttpClientService.GetResourceAsync<UsuarioDTO>(this.BaseAddressAPI, $"usuarios/{id}");
        }

        public async Task<IResponse<UsuarioDTO>> GetUsuarioByIdRed(string idRed, bool gdiAuth = true)
        {
            if (!gdiAuth)
            {
                var azureAdToken = await this.AzureAdAuthClient.GetTokenAsync();
                return await this.HttpClientService.GetResourceAsync<UsuarioDTO>(this.BaseAddressAPI, $"usuarios/azureAD/idred/{idRed}", authToken: azureAdToken)!;
            }

            return await this.HttpClientService.GetResourceAsync<UsuarioDTO>(this.BaseAddressAPI, $"usuarios/idred/{idRed}")!;
        }

        public async Task<IPaginatedResponse<UsuarioAcotadoDTO>> GetUsuariosByIds(GetByIdsListDTO dto, bool gdiAuth = true)
        {
            var cachedUsuarios = new List<UsuarioAcotadoDTO>();
            var missingIds = new List<int>();

            foreach (var id in dto.Ids.Distinct())
            {
                if (UsuarioAcotadoCache.TryGetValue($"Usuario_{id}", out UsuarioAcotadoDTO? cached) && cached is not null)
                {
                    cachedUsuarios.Add(cached);
                }
                else
                {
                    missingIds.Add(id);
                }
            }

            if (missingIds.Count == 0)
            {
                return PaginatedResponse<UsuarioAcotadoDTO>.Success(
                    cachedUsuarios,
                    new PaginationData { TotalCount = cachedUsuarios.Count, PageNumber = dto.PageNumber, PageSize = dto.PageSize });
            }

            var batchDto = new GetByIdsListDTO { Ids = missingIds, PageNumber = 1, PageSize = missingIds.Count };
            IPaginatedResponse<UsuarioAcotadoDTO> response;

            if (!gdiAuth)
            {
                var azureAdToken = await this.AzureAdAuthClient.GetTokenAsync();
                response = await this.HttpClientService.PostResourcesAsync<GetByIdsListDTO, UsuarioAcotadoDTO>(this.BaseAddressAPI, batchDto, "usuarios/azureAD/batch", authToken: azureAdToken)!;
            }
            else
            {
                response = await this.HttpClientService.PostResourcesAsync<GetByIdsListDTO, UsuarioAcotadoDTO>(this.BaseAddressAPI, batchDto, "usuarios/batch");
            }

            if (response is null || !response.IsSuccess || response.Data is null)
            {
                return response!;
            }

            var fetchedUsuarios = response.Data.Where(u => u != null).Select(u => u!).ToList();

            foreach (var usuario in fetchedUsuarios)
            {
                UsuarioAcotadoCache.Set($"Usuario_{usuario.Id}", usuario, this.CacheDuration);
            }

            var allUsuarios = cachedUsuarios.Concat(fetchedUsuarios).ToList();
            return PaginatedResponse<UsuarioAcotadoDTO>.Success(
                allUsuarios,
                new PaginationData { TotalCount = allUsuarios.Count, PageNumber = dto.PageNumber, PageSize = dto.PageSize });
        }

        public async Task<IPaginatedResponse<UsuarioAcotadoDTO>> GetUsuariosByRolesId(GetByIdsListDTO dto, bool gdiAuth = true)
        {
            var parameters = QueryHelpers.ToQueryKeyValuePairs(dto);

            if (!gdiAuth)
            {
                var azureAdToken = await this.AzureAdAuthClient.GetTokenAsync();
                return await this.HttpClientService.GetResourcesAsync<UsuarioAcotadoDTO>(this.BaseAddressAPI, "usuarios/azureAD/por-rol/batch", parameters!, azureAdToken)!;
            }

            return await this.HttpClientService.GetResourcesAsync<UsuarioAcotadoDTO>(this.BaseAddressAPI, "usuarios/por-rol/batch", parameters!);
        }

        public async Task<IPaginatedResponse<UsuariosDTO>> GetUsuarios(GetPaginatedUsuariosDTO dto, bool gdiAuth = true)
        {
            var parameters = QueryHelpers.ToQueryKeyValuePairs(dto);

            if (!gdiAuth)
            {
                var azureAdToken = await this.AzureAdAuthClient.GetTokenAsync();
                return await this.HttpClientService.GetResourcesAsync<UsuariosDTO>(this.BaseAddressAPI, "usuarios/azureAD", parameters!, azureAdToken)!;
            }

            return await this.HttpClientService.GetResourcesAsync<UsuariosDTO>(this.BaseAddressAPI, "usuarios", parameters!)!;
        }

        public async Task<IResponse> AgregarRolesAUsuarios(AgregarRolesUsuarioDTO input)
        {
            return await this.HttpClientService.PatchResourceAsync(this.BaseAddressAPI, input, "usuarios/agregar-rol")!;
        }

        public async Task<IResponse> EliminarRolUsuariosBatch(UpdateUsuarioRolesBatchDTO input)
        {
            return await this.HttpClientService.PatchResourceAsync(this.BaseAddressAPI, input, "usuarios/eliminar-rol-usuarios")!;
        }

        public async Task<IResponse> EliminarRolUsuariosBatchAzureAD(UpdateUsuarioRolesBatchDTO input)
        {
            var azureAdToken = await this.AzureAdAuthClient.GetTokenAsync();

            return await this.HttpClientService.PatchResourceAsync(this.BaseAddressAPI, input, "usuarios/azureAD/eliminar-rol-usuarios", authToken: azureAdToken)!;
        }

        public async Task<IPaginatedResponse<ReporteUsuariosRolesSRCDTO>> GetUsuariosRolesSRC(GetPaginatedReporteUsuariosRolesSRCDTO dto, bool gdiAuth = true)
        {
            var parameters = QueryHelpers.ToQueryKeyValuePairs(dto);

            if (!gdiAuth)
            {
                var azureAdToken = await this.AzureAdAuthClient.GetTokenAsync();
                return await this.HttpClientService.GetResourcesAsync<ReporteUsuariosRolesSRCDTO>(this.BaseAddressAPI, $"usuarios/azureAD/reportes/usuarios-roles-src", parameters!, azureAdToken)!;
            }

            return await this.HttpClientService.GetResourcesAsync<ReporteUsuariosRolesSRCDTO>(this.BaseAddressAPI, "usuarios/reportes/usuarios-roles-src", parameters!)!;
        }

        public async Task<IPaginatedResponse<RolDetailsDTO>> GetRolesByIds(GetByIdsListDTO dto, bool gdiAuth = true)
        {
            var parameters = QueryHelpers.ToQueryKeyValuePairs(dto);

            if (!gdiAuth)
            {
                var azureAdToken = await this.AzureAdAuthClient.GetTokenAsync();
                return await this.HttpClientService.GetResourcesAsync<RolDetailsDTO>(this.BaseAddressAPI, "usuarios/azureAD/roles/batch", parameters!, azureAdToken)!;
            }

            return await this.HttpClientService.GetResourcesAsync<RolDetailsDTO>(this.BaseAddressAPI, "usuarios/roles/batch", parameters!)!;
        }

        public async Task<IResponse<List<int>>> GetPermisosByRolesIds(GetByIdsListDTO dto, bool gdiAuth = true)
        {
            var parameters = QueryHelpers.ToQueryKeyValuePairs(dto);

            if (!gdiAuth)
            {
                var azureAdToken = await this.AzureAdAuthClient.GetTokenAsync();
                return await this.HttpClientService.GetResourceAsync<List<int>>(this.BaseAddressAPI, "usuarios/azureAD/permisos-por-roles", parameters!, azureAdToken)!;
            }

            return await this.HttpClientService.GetResourceAsync<List<int>>(this.BaseAddressAPI, "usuarios/permisos-por-roles", parameters!)!;
        }

        public async Task<IResponse<bool>> ValidarCompatibilidadRoles(ValidarCompatibilidadRolesDTO dto, bool gdiAuth = true)
        {
            var parameters = QueryHelpers.ToQueryKeyValuePairs(dto);

            if (!gdiAuth)
            {
                var azureAdToken = await this.AzureAdAuthClient.GetTokenAsync();
                return await this.HttpClientService.GetResourceAsync<bool>(this.BaseAddressAPI, "usuarios/validar-compatibilidad-roles", parameters!, azureAdToken)!;
            }

            return await this.HttpClientService.GetResourceAsync<bool>(this.BaseAddressAPI, "usuarios/validar-compatibilidad-roles", parameters!)!;
        }

        public async Task<IResponse<UsuarioDTO>> AsegurarUsuarioInterno(AsegurarUsuarioInternoDTO dto)
        {
            var azureAdToken = await this.AzureAdAuthClient.GetTokenAsync();

            return await this.HttpClientService.PostResourceAsync<AsegurarUsuarioInternoDTO, UsuarioDTO>(
                this.BaseAddressAPI,
                dto,
                "usuarios/azureAD/internos/asegurar",
                authToken: azureAdToken)!;
        }

        public async Task<IPaginatedResponse<UsuariosDTO>> BuscarAdUsuariosExistentes(string palabraClave, bool gdiAuth = true)
        {
            var parameters = QueryHelpers.ToQueryKeyValuePairs(new { palabraClave });

            if (!gdiAuth)
            {
                var azureAdToken = await this.AzureAdAuthClient.GetTokenAsync();
                return await this.HttpClientService.GetResourcesAsync<UsuariosDTO>(this.BaseAddressAPI, "ad-usuarios/existentes", parameters!, azureAdToken)!;
            }

            return await this.HttpClientService.GetResourcesAsync<UsuariosDTO>(this.BaseAddressAPI, "ad-usuarios/existentes", parameters!)!;
        }

        public async Task<IResponse<List<int>>> GetUsuariosIds(GetUsuariosIdsDTO dto, bool gdiAuth = true)
        {
            if (!gdiAuth)
            {
                var azureAdToken = await this.AzureAdAuthClient.GetTokenAsync();
                return await this.HttpClientService.PostResourceAsync<GetUsuariosIdsDTO, List<int>>(this.BaseAddressAPI, dto, "usuarios/azureAD/ids", authToken: azureAdToken)!;
            }

            return await this.HttpClientService.PostResourceAsync<GetUsuariosIdsDTO, List<int>>(this.BaseAddressAPI, dto, "usuarios/ids")!;
        }

        public async Task<IResponse<List<UsuarioDirectorioDTO>>> BuscarUsuariosDirectorio(BuscarUsuarioDirectorioDTO dto, bool gdiAuth = true)
        {
            var parameters = QueryHelpers.ToQueryKeyValuePairs(dto);

            if (!gdiAuth)
            {
                var azureAdToken = await this.AzureAdAuthClient.GetTokenAsync();
                return await this.HttpClientService.GetResourceAsync<List<UsuarioDirectorioDTO>>(this.BaseAddressAPI, "usuarios/azureAD/directorio/buscar", parameters!, azureAdToken)!;
            }

            return await this.HttpClientService.GetResourceAsync<List<UsuarioDirectorioDTO>>(this.BaseAddressAPI, "usuarios/directorio/buscar", parameters!)!;
        }

        public async Task<IResponse<UsuarioDirectorioResueltoDTO>> ResolverUsuarioConRol(ResolverUsuarioConRolDTO dto, bool gdiAuth = true)
        {
            if (!gdiAuth)
            {
                var azureAdToken = await this.AzureAdAuthClient.GetTokenAsync();
                return await this.HttpClientService.PostResourceAsync<ResolverUsuarioConRolDTO, UsuarioDirectorioResueltoDTO>(this.BaseAddressAPI, dto, "usuarios/azureAD/directorio/resolver-con-rol", authToken: azureAdToken)!;
            }

            return await this.HttpClientService.PostResourceAsync<ResolverUsuarioConRolDTO, UsuarioDirectorioResueltoDTO>(this.BaseAddressAPI, dto, "usuarios/directorio/resolver-con-rol")!;
        }
    }
}
