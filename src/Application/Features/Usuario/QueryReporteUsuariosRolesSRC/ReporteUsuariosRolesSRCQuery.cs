namespace UsuariosAPI.Application.Features.Usuario.QueryReporteUsuariosRolesSRC
{
    using UsuariosAPI.Application.Abstractions.Messaging;
    using UsuariosAPI.SDK.Contracts.DTOs.Usuario;

    public sealed record ReporteUsuariosRolesSRCQuery(GetPaginatedReporteUsuariosRolesSRCDTO dto, bool gdiAuth) : IQueryPaginated<ReporteUsuariosRolesSRCDTO>
    {
    }
}
