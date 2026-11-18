namespace UsuariosAPI.Application.Features.DelegacionTemporal.QueryExportDelegacionTemporal
{
    using APICore.Responses.DTOs;
    using UsuariosAPI.Application.Abstractions.Messaging;
    using UsuariosAPI.SDK.Contracts.DTOs.DelegacionTemporal;

    public sealed record ExportDelegacionTemporalQuery(GetDelegacionTemporalPaginatedDTO dto) : IQuery<FileResponseDTO>;
}
