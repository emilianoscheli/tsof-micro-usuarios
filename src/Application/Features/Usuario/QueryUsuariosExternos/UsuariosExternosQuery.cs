namespace UsuariosAPI.Application.Features.Usuario.QueryUsuariosExternos
{
    using UsuariosAPI.Application.Abstractions.Messaging;
    using UsuariosAPI.SDK.Contracts.DTOs.Usuario;

    public sealed record UsuariosExternosQuery(GetPaginatedUsuariosExternosDTO dto, bool gdiAuth = true) : IQueryPaginated<UsuariosExternosDTO>;
}
