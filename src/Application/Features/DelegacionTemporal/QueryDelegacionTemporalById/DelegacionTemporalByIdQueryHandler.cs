namespace UsuariosAPI.Application.Features.DelegacionTemporal.QueryDelegacionTemporalById
{
    using APICore.Interfaces.Responses;
    using APICore.Responses;
    using AutoMapper;
    using UsuariosAPI.Application.Abstractions.Messaging;
    using UsuariosAPI.Application.Interfaces;
    using UsuariosAPI.Domain.Abstractions;
    using UsuariosAPI.Domain.Abstractions.Errors;
    using UsuariosAPI.Domain.DelegacionTemporal;
    using UsuariosAPI.SDK.Contracts.DTOs.DelegacionTemporal;

    public class DelegacionTemporalByIdQueryHandler : IQueryHandler<DelegacionTemporalByIdQuery, DelegacionTemporalDTO>
    {
        public DelegacionTemporalByIdQueryHandler(
           IUnitOfWork unitOfwork,
           IMapper mapper,
           IMotivosDelegacionServiceConsumer motivosDelegacionServiceConsumer)
        {
            this.UnitOfWork = unitOfwork;
            this.AutoMapper = mapper;
            this.MotivosDelegacionServiceConsumer = motivosDelegacionServiceConsumer;
        }

        private IUnitOfWork UnitOfWork { get; }

        private IMapper AutoMapper { get; }

        private IMotivosDelegacionServiceConsumer MotivosDelegacionServiceConsumer { get; }

        public async Task<IResponse<DelegacionTemporalDTO>> Handle(DelegacionTemporalByIdQuery request, CancellationToken cancellationToken)
        {
            var entidad = await this.UnitOfWork.DelegacionTemporalRepository.GetByIdWithUsuariosDataAsync(request.dto.Id, cancellationToken);

            if (entidad is null)
            {
                return Response.Failure<DelegacionTemporalDTO>(UsuariosError.EntidadNoEncontrada(nameof(DelegacionTemporal)));
            }

            var entidadResponse = this.AutoMapper.Map<DelegacionTemporalDTO>(entidad);

            if (entidadResponse.MotivoId != null && entidadResponse.MotivoId > 0)
            {
                var (motivo, _) = await this.MotivosDelegacionServiceConsumer.ValidateMotivoDelegacion(entidadResponse.MotivoId!.Value);
                entidadResponse.Motivo = motivo?.NombreMotivo ?? string.Empty;
            }

            return Response.Success(entidadResponse);
        }
    }
}
