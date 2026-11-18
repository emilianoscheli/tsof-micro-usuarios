namespace UsuariosAPI.Application.Features.DelegacionTemporal.QueryCantidadRecursosAfectadosPorMotivoDelegacion
{
    using APICore.Requests.DTOs;
    using APICore.Responses.DTOs;
    using UsuariosAPI.Application.Abstractions.Messaging;

    public sealed record CantidadRecursosAfectadosPorMotivoDelegacionQuery(GetByIdDTO dto) : IQuery<CantidadRecursosAfectadosDTO>
    {
    }
}
