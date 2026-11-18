namespace UsuariosAPI.Application.Features.Usuario.QueryUsuariosByRolIdBatch
{
    using APICore.Requests.DTOs;
    using UsuariosAPI.Application.Abstractions.Messaging;
    using UsuariosAPI.SDK.Contracts.DTOs.Usuario;

    public sealed record UsuariosByRolIdBatchQuery(GetByIdsListDTO dto) : IQueryPaginated<UsuarioAcotadoDTO>;
}
