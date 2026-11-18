namespace UsuariosAPI.Application.Features.Rol.QueryRoles
{
    using APICore.Responses.DTOs;
    using UsuariosAPI.Application.Abstractions.Messaging;
    using UsuariosAPI.SDK.Contracts.DTOs.UsuarioRol;

    public sealed record RolesQuery(GetRolDTO dto) : IQuery<List<RolDTO>>
    {
    }
}
