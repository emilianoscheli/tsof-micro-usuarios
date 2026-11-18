namespace UsuariosAPI.Application.Features.Usuario.QueryUsuariosSelect
{
    using UsuariosAPI.Application.Abstractions.Messaging;
    using UsuariosAPI.SDK.Contracts.DTOs.Usuario;

    public sealed record UsuariosSelectQuery(GetPaginatedUsuariosDTO dto) : IQueryPaginated<UsuarioAcotadoDTO>
    {
    }
}
