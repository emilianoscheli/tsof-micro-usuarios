namespace UsuariosAPI.Application.Features.DelegacionTemporal.QueryCantidadRecursosAfectadosPorMotivoDelegacion
{
    using APICore.Interfaces.Responses;
    using APICore.Responses;
    using APICore.Responses.DTOs;
    using UsuariosAPI.Application.Abstractions.Messaging;
    using UsuariosAPI.Domain.Abstractions;

    public sealed class CantidadRecursosAfectadosPorMotivoDelegacionQueryHandler : IQueryHandler<CantidadRecursosAfectadosPorMotivoDelegacionQuery, CantidadRecursosAfectadosDTO>
    {
        public CantidadRecursosAfectadosPorMotivoDelegacionQueryHandler(
            IUnitOfWork unitOfWork)
        {
            this.UnitOfWork = unitOfWork;
        }

        private IUnitOfWork UnitOfWork { get; }

        public async Task<IResponse<CantidadRecursosAfectadosDTO>> Handle(CantidadRecursosAfectadosPorMotivoDelegacionQuery request, CancellationToken cancellationToken)
        {
            var registrosAsociados = await this.UnitOfWork.DelegacionTemporalRepository.GetByMotivoDelegacionId(request.dto.Id, cancellationToken);
            var cantidad = 0;

            if (registrosAsociados is not null && registrosAsociados.Count() > 0)
            {
                cantidad = registrosAsociados.Count();
            }

            var dto = new CantidadRecursosAfectadosDTO { RecursosAfectados = cantidad };

            return Response.Success(dto);
        }
    }
}
