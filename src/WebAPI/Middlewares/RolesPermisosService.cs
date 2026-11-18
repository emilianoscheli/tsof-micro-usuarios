namespace UsuariosAPI.WebAPI.Middlewares
{
    using System.IdentityModel.Tokens.Jwt;
    using APICore.Interfaces.Services;
    using APICore.Responses.DTOs;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.DependencyInjection;
    using UsuariosAPI.Application.Interfaces.CommonServices;

    public sealed class RolesPermisosService : IRolesPermisosService
    {
        private readonly IServiceProvider serviceProvider;

        public RolesPermisosService(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        public async Task<UsuarioRolesPermisosDTO?> GetRolesPermisosByIdRed(string idRed)
        {
            try
            {
                using var scope = this.serviceProvider.CreateScope();
                var service = scope.ServiceProvider;

                var serviceLocalUsuarios = service.GetRequiredService<IUsuariosRolesPermisosService>();

                return await serviceLocalUsuarios.GetRolesPermisosUsuarios(idRed);
            }
            catch
            {
                return null;
            }
        }

        public bool IsEndpointAuthorized(HttpContext httpContext)
        {
            var endpoint = httpContext.GetEndpoint();
            if (endpoint is null)
            {
                return false; // Si no hay endpoint, no autorizar
            }

            var authorizeAttribute = endpoint.Metadata.GetMetadata<AuthorizeAttribute>();
            if (authorizeAttribute is null)
            {
                return false;
            }

            var authToken = httpContext.Request.Headers["Authorization"].ToString().Replace("Bearer", string.Empty).Trim();

            if (!string.IsNullOrEmpty(authToken))
            {
                var handler = new JwtSecurityTokenHandler();
                if (handler.CanReadToken(authToken))
                {
                    try
                    {
                        var jsonToken = handler.ReadJwtToken(authToken);

                        if (jsonToken.Issuer?.Contains("sts.windows.net") == true ||
                            jsonToken.Issuer?.Contains("login.microsoftonline.com") == true)
                        {
                            return false;
                        }

                        if (!jsonToken.Payload.ContainsKey("token_details") &&
                                !(httpContext.User.Identity?.IsAuthenticated == true &&
                                    jsonToken.Issuer == httpContext.RequestServices.GetRequiredService<IConfiguration>()["IBMVerify:Issuer"] &&
                                    httpContext.RequestServices.GetRequiredService<IConfiguration>()["IdentityProvider"] == "IBMVerify"))
                        {
                            return false;
                        }
                    }
                    catch
                    {
                        return false;
                    }
                }
            }

            return true; // Solo procesar tokens GDI válidos con [Authorize]
        }
    }
}
