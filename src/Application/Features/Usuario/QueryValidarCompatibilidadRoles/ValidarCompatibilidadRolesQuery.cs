namespace UsuariosAPI.Application.Features.Usuario.QueryValidarCompatibilidadRoles
{
    using UsuariosAPI.Application.Abstractions.Messaging;
    using UsuariosAPI.SDK.Contracts.DTOs.CompatibilidadRol;

    public record ValidarCompatibilidadRolesQuery(ValidarCompatibilidadRolesDTO dto) : IQuery<bool>;
}
