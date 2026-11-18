namespace UsuariosAPI.Application.Features.DelegacionTemporal.QueryArchivoRecursosAfectacosPorMotivoDelegacion
{
    using APICore.Requests.DTOs;
    using UsuariosAPI.Application.Abstractions.Messaging;
    using UsuariosAPI.SDK.Contracts.DTOs.DelegacionTemporal;

    public sealed record ArchivoRecursosAfectadosPorMotivoDelegacionQuery(GetByIdDTO dto) : IQuery<List<ExcelCantidadRegistrosAsociadosMotivoDelegacionDTO>>
    {
    }
}
