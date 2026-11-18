namespace UsuariosAPI.Application.Features.DelegacionTemporal.QueryArchivoRecursosAfectacosPorMotivoDelegacion
{
    using APICore.Interfaces.Responses;
    using APICore.Responses;
    using APICore.Responses.DTOs;
    using AutoMapper;
    using UsuariosAPI.Application.Abstractions.Messaging;
    using UsuariosAPI.Application.Interfaces;
    using UsuariosAPI.Domain.Abstractions;
    using UsuariosAPI.SDK.Contracts.DTOs.DelegacionTemporal;

    public sealed class ArchivoRecursosAfectadosPorMotivoDelegacionQueryHandler : IQueryHandler<ArchivoRecursosAfectadosPorMotivoDelegacionQuery, List<ExcelCantidadRegistrosAsociadosMotivoDelegacionDTO>>
    {
        public ArchivoRecursosAfectadosPorMotivoDelegacionQueryHandler(
            IMapper mapper,
            IUnitOfWork unitOfWork)
        {
            this.AutoMapper = mapper;
            this.UnitOfWork = unitOfWork;
        }

        private IUnitOfWork UnitOfWork { get; }

        private IMapper AutoMapper { get; }

        public async Task<IResponse<List<ExcelCantidadRegistrosAsociadosMotivoDelegacionDTO>>> Handle(ArchivoRecursosAfectadosPorMotivoDelegacionQuery request, CancellationToken cancellationToken)
        {
            var registrosAsociados = await this.UnitOfWork.DelegacionTemporalRepository.GetByMotivoDelegacionId(request.dto.Id, cancellationToken);

            var dtoFinal = this.AutoMapper.Map<List<ExcelCantidadRegistrosAsociadosMotivoDelegacionDTO>>(registrosAsociados);

            return Response.Success(dtoFinal);
        }
    }
}
