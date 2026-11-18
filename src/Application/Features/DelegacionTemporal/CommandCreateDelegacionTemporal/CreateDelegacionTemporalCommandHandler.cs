namespace UsuariosAPI.Application.Features.DelegacionTemporal.CommandCreateDelegacionTemporal
{
    using APICore.Enums;
    using APICore.Interfaces.Responses;
    using APICore.Responses;
    using AutoMapper;
    using UsuariosAPI.Application.Abstractions.Messaging;
    using UsuariosAPI.Application.Interfaces.CommonServices;
    using UsuariosAPI.Domain.Abstractions;
    using UsuariosAPI.Domain.Abstractions.Errors;
    using UsuariosAPI.Domain.DelegacionTemporal;
    using UsuariosAPI.Domain.Usuario;
    using UsuariosAPI.SDK.Contracts.DTOs.DelegacionTemporal;

    public class CreateDelegacionTemporalCommandHandler : ICommandHandler<CreateDelegacionTemporalCommand>
    {
        public CreateDelegacionTemporalCommandHandler(
           IUnitOfWork unitOfWork,
           IMapper mapper,
           IAuthenticatedUserService authenticatedUserService)
        {
            this.UnitOfWork = unitOfWork;
            this.AutoMapper = mapper;
            this.AuthenticatedUserService = authenticatedUserService;
        }

        private IUnitOfWork UnitOfWork { get; }

        private IMapper AutoMapper { get; }

        private IAuthenticatedUserService AuthenticatedUserService { get; }

        public async Task<IResponse> Handle(CreateDelegacionTemporalCommand request, CancellationToken cancellationToken)
        {
            var esAdmin = this.AuthenticatedUserService.AuthenticatedUserEsRol(RolesEnum.Administrador);
            var userId = this.AuthenticatedUserService.GetAuthenticatedUserId();
            var delegador = await this.UnitOfWork.UsuarioSRCRepository.GetByIdRedWithDependenciesAsync(request.dto.DelegadorIdRed!, cancellationToken);
            var delegado = await this.UnitOfWork.UsuarioSRCRepository.GetByIdRedWithDependenciesAsync(request.dto.DelegadoIdRed!, cancellationToken);

            if (delegado == null || delegador == null)
            {
                return Response.Failure<DelegacionTemporalDTO>(UsuariosError.EntidadNoEncontrada(nameof(UsuarioSRC)));
            }

            if (!esAdmin && delegador.Id != userId)
            {
                return Response.Failure<DelegacionTemporalDTO>(UsuariosError.OperacionNoPermitida("Solo puedes delegar tus propias facultades"));
            }

            // Un rol con estudio auditor o contratista no puede delegar roles.
            var delegadorTienePermisos = !delegador!.Roles!.Any(x => x.RolId == (int)RolesEnum.Contratista || x.RolId == (int)RolesEnum.EstudioAuditor);

            if (!delegadorTienePermisos)
            {
                return Response.Failure<DelegacionTemporalDTO>(UsuariosError.OperacionNoPermitida("No tienes los permisos para realizar la accion"));
            }

            var existeSubDelegacion = await this.UnitOfWork.DelegacionTemporalRepository
                                    .ExisteSuperposicionComoDelegadoAsync(
                                        delegador.Id,
                                        request.dto.FechaDesde,
                                        request.dto.FechaHasta,
                                        cancellationToken);

            if (existeSubDelegacion)
            {
                return Response.Failure<DelegacionTemporalDTO>(
                    UsuariosError.OperacionNoPermitida(
                        "El usuario seleccionado no puede sub-delegar facultades mientras sea Delegado vigente en el mismo período."));
            }

            var entidad = this.AutoMapper.Map<DelegacionTemporal>(request.dto);

            entidad.UsuarioDelegadorId = delegador.Id;
            entidad.UsuarioDelegadoId = delegado.Id;

            // Valida compatibilidad de roles entre usuario delegado y delegador
            if (delegado.Roles != null && delegado.Roles.Count > 0 && delegador.Roles != null && delegador.Roles.Count > 0)
            {
                var rolesActualesIds = delegado.Roles.Select(d => d.RolId).ToList();
                var rolesDestinoIds = delegador.Roles.Select(d => d.RolId).ToList();

                var rolesIncompatibles = this.UnitOfWork.CompatibilidadRolRepository
                    .GetRolesIncompatibles(rolesActualesIds, rolesDestinoIds);

                if (rolesIncompatibles.Count > 0)
                {
                    return Response.Failure<DelegacionTemporalDTO>(
                    UsuariosError.OperacionNoPermitida(
                        "El usuario delegado tiene roles incompatibles con el delegante."));
                }
            }

            var rolesAgregar = await this.UnitOfWork.DelegacionTemporalRepository.SyncRoles(delegador.Id, delegado.Id, cancellationToken);

            if (rolesAgregar is { Count: > 0 })
            {
                entidad.RolesTemporales = rolesAgregar
                    .Select(id => new RolTemporal
                    {
                        RolId = id,
                        DelegacionTemporalId = 0,
                    }).ToList();
            }

            await this.UnitOfWork.DelegacionTemporalRepository.AddAsync(entidad, cancellationToken);

            if (entidad.RolesTemporales != null)
            {
                foreach (var rol in entidad.RolesTemporales)
                {
                    rol.DelegacionTemporalId = (int)entidad.Id!;
                }
            }

            await this.UnitOfWork.SaveAsync(cancellationToken);

            return Response.Success();
        }
    }
}
