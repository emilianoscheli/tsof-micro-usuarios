namespace UsuariosAPI.Application.Features.Usuario.QueryUsuarios
{
    using UsuariosAPI.Application.Abstractions.Messaging;
    using UsuariosAPI.SDK.Contracts.DTOs.Usuario;

    public sealed record UsuariosQuery(GetPaginatedUsuariosDTO dto, bool gdiAuth = true) : IQueryPaginated<UsuariosDTO>
    {
    }
}
