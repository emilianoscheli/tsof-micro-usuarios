namespace UsuariosAPI.Application.Features.DelegacionTemporal.QueryDelegacionTemporalById
{
    using APICore.Requests.DTOs;
    using UsuariosAPI.Application.Abstractions.Messaging;
    using UsuariosAPI.SDK.Contracts.DTOs.DelegacionTemporal;

    public sealed record DelegacionTemporalByIdQuery(GetByIdDTO dto) : IQuery<DelegacionTemporalDTO>;
}
