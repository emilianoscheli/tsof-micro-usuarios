namespace UsuariosAPI.Application.Features.Usuario.QueryUsuariosByIdBatch
{
    using APICore.Requests.DTOs;
    using UsuariosAPI.Application.Abstractions.Messaging;
    using UsuariosAPI.SDK.Contracts.DTOs.Usuario;

    public sealed record UsuariosByIdBatchQuery(GetByIdsListDTO dto) : IQueryPaginated<UsuarioAcotadoDTO>;
}
