namespace UsuariosAPI.Application.Interfaces.CommonServices
{
    using APICore.Enums;

    public interface IAuthenticatedUserService
    {
        int GetAuthenticatedUserId();

        List<int> GetAuthenticatedUserRoles();

        int GetAuthenticatedUserContratistaId();

        int GetAuthenticatedUserEstudioAuditorId();

        string GetAuthenticatedUserName();

        bool AuthenticatedUserEsRol(RolesEnum rol);

        bool AuthenticatedUserTieneAlgunRol(List<RolesEnum> rolesRequeridos);
    }
}
