namespace UsuariosAPI.Application.Features.DelegacionTemporal.CommandDesactivarDelegacionTemporal
{
    using System.Linq;
    using System.Threading.Tasks;
    using APICore.Interfaces.Responses;
    using APICore.Responses;
    using AutoMapper;
    using UsuariosAPI.Application.Abstractions.Messaging;
    using UsuariosAPI.Domain.Abstractions;
    using UsuariosAPI.Domain.Abstractions.Errors;
    using UsuariosAPI.Domain.DelegacionTemporal;

    public sealed class DesactivarDelegacionTemporalCommandHandler : ICommandHandler<DesactivarDelegacionTemporalCommand>
    {
        public DesactivarDelegacionTemporalCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            this.UnitOfWork = unitOfWork;
            this.AutoMapper = mapper;
        }

        private IUnitOfWork UnitOfWork { get; }

        private IMapper AutoMapper { get; }

        public async Task<IResponse> Handle(DesactivarDelegacionTemporalCommand request, CancellationToken cancellationToken)
        {
            var entidad = await this.UnitOfWork.DelegacionTemporalRepository.GetByIdAsync(request.dto.Id, cancellationToken);

            if (entidad is null)
            {
                return Response.Failure(UsuariosError.EntidadNoEncontrada(nameof(DelegacionTemporal)));
            }

            var result = entidad.Desactivar();

            if (result is null || !result.IsSuccess)
            {
                return Response.Failure(result?.Errors?.ToList() ?? [Error.DesactivationFailed]);
            }

            if (entidad.RolesTemporales != null)
            {
                var roles = entidad.RolesTemporales.ToList();

                foreach (var rol in roles)
                {
                    entidad.RolesTemporales!.Remove(rol);
                    this.UnitOfWork.RolTemporalRepository.Remove(rol);
                }
            }

            await this.UnitOfWork.SaveAsync(cancellationToken: cancellationToken, logTecnicoComment: request.dto.Motivo);

            return Response.Success();
        }
    }
}
