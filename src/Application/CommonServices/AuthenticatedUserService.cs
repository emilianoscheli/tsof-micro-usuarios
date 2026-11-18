namespace UsuariosAPI.Application.CommonServices
{
    using APICore.Enums;
    using APICore.Extensions;
    using Microsoft.AspNetCore.Http;
    using UsuariosAPI.Application.Interfaces.CommonServices;

    public class AuthenticatedUserService : IAuthenticatedUserService
    {
        public AuthenticatedUserService(
            IHttpContextAccessor httpContextAccessor)
        {
            this.HttpContextAccessor = httpContextAccessor;
        }

        private IHttpContextAccessor HttpContextAccessor { get; }

        public int GetAuthenticatedUserId()
        {
            return this.HttpContextAccessor.HttpContext.GetUsuarioId();
        }

        public List<int> GetAuthenticatedUserRoles()
        {
            return this.HttpContextAccessor.HttpContext.GetRolesIdUsuario();
        }

        public int GetAuthenticatedUserContratistaId()
        {
            return this.HttpContextAccessor.HttpContext.GetContratistaId();
        }

        public int GetAuthenticatedUserEstudioAuditorId()
        {
            return this.HttpContextAccessor.HttpContext.GetEstudioAuditorId();
        }

        public string GetAuthenticatedUserName()
        {
            return this.HttpContextAccessor.HttpContext.GetNombreUsuario();
        }

        public bool AuthenticatedUserEsRol(RolesEnum rol)
        {
            return this.HttpContextAccessor.HttpContext.TieneRolId((int)rol);
        }

        public bool AuthenticatedUserTieneAlgunRol(List<RolesEnum> rolesRequeridos)
        {
            var rolesActuales = this.HttpContextAccessor.HttpContext.GetRolesIdUsuario() ?? [];

            return rolesRequeridos.Select(r => (int)r).Any(rolesActuales.Contains);
        }
    }
}
