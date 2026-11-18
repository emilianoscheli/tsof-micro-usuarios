namespace UsuariosAPI.Application.Features.DelegacionTemporal.QueryDelegacionTemporal
{
    using UsuariosAPI.Application.Abstractions.Messaging;
    using UsuariosAPI.SDK.Contracts.DTOs.DelegacionTemporal;

    public sealed record DelegacionTemporalQuery(GetDelegacionTemporalPaginatedDTO dto) : IQueryPaginated<DelegacionTemporalDTO>;
}
