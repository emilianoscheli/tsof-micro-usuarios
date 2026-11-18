namespace UsuariosAPI.Application.Features.DelegacionTemporal.QueryReporteDelegacionTemporal
{
    using UsuariosAPI.Application.Abstractions.Messaging;
    using UsuariosAPI.SDK.Contracts.DTOs.DelegacionTemporal;

    public sealed record ReporteDelegacionTemporalQuery(GetPaginatedReporteDelegacionTemporalDTO dto, bool gdiAuth) : IQueryPaginated<ReporteDelegacionTemporalDTO>
    {
    }
}