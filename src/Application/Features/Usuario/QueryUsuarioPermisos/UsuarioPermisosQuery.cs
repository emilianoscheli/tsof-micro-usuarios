namespace UsuariosAPI.Application.Features.Usuario.QueryUsuarioPermisos
{
    using APICore.Responses.DTOs;
    using UsuariosAPI.Application.Abstractions.Messaging;
    using UsuariosAPI.SDK.Contracts.DTOs.UsuarioRol;

    public sealed record UsuarioPermisosQuery(GetPermisosUsuarioDTO dto) : IQuery<UsuarioRolesPermisosDTO>;
}