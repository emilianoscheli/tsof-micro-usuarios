namespace UsuariosAPI.Application.Features.Rol.QueryRolesByIdBatch
{
    using APICore.Requests.DTOs;
    using UsuariosAPI.Application.Abstractions.Messaging;
    using UsuariosAPI.SDK.Contracts.DTOs.Usuario;

    public sealed record RolesByIdBatchQuery(GetByIdsListDTO dto) : IQueryPaginated<RolDetailsDTO>
    {
    }
}
