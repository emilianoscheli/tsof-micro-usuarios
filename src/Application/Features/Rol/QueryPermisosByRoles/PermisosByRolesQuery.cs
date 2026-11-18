namespace UsuariosAPI.Application.Features.Rol.QueryPermisosByRoles
{
    using APICore.Requests.DTOs;
    using UsuariosAPI.Application.Abstractions.Messaging;

    public sealed record PermisosByRolesQuery(GetByIdsListDTO dto) : IQuery<List<int>>
    {
    }
}
