namespace UsuariosAPI.Application.Features.Usuario.CommandDesactivarUsuarioExterno
{
    using System.Threading;
    using System.Threading.Tasks;
    using APICore.Enums;
    using APICore.Interfaces.Responses;
    using APICore.Responses;
    using IntegracionesAPI.SDK.Contracts.DTOs.GDIE;
    using UsuariosAPI.Application.Abstractions.Messaging;
    using UsuariosAPI.Application.Interfaces;
    using UsuariosAPI.Domain.Abstractions;
    using UsuariosAPI.Domain.Abstractions.Errors;
    using UsuariosAPI.Domain.Usuario;
    using UsuariosAPI.Domain.UsuarioRol;

    public class DesactivarUsuarioExternoCommandHandler : ICommandHandler<DesactivarUsuarioExternoCommand>
    {
        public DesactivarUsuarioExternoCommandHandler(
            IUnitOfWork unitOfWork,
            IGDIEServiceConsumer gdieServiceConsumer)
        {
            this.UnitOfWork = unitOfWork;
            this.GDIEServiceConsumer = gdieServiceConsumer;
        }

        private IUnitOfWork UnitOfWork { get; }

        private IGDIEServiceConsumer GDIEServiceConsumer { get; }

        public async Task<IResponse> Handle(DesactivarUsuarioExternoCommand request, CancellationToken cancellationToken)
        {
            var dto = request.dto;

            var usuario = await this.UnitOfWork.UsuarioSRCRepository
                .GetByIdWithAllDataAsync(dto.UsuarioId, cancellationToken);

            if (usuario is null)
            {
                return Response.Failure(UsuariosError.EntidadNoEncontrada(nameof(UsuarioSRC)));
            }

            if (!usuario.EsExterno)
            {
                return Response.Failure(UsuariosError.UsuarioNoEsExterno);
            }

            if (!usuario.Activo)
            {
                return Response.Failure(UsuariosError.UsuarioExternoYaInactivo);
            }

            if (string.IsNullOrWhiteSpace(usuario.IdRed))
            {
                return Response.Failure(Error.GenericError("El usuario externo no tiene UID/IdRed cargado para consultar GDIE."));
            }

            var (gdieExistente, gdieExistenteError) = await this.GDIEServiceConsumer
                .ObtenerUsuarioPorUserIdAsync(usuario.IdRed.Trim());

            if (gdieExistenteError is not null)
            {
                return Response.Failure(gdieExistenteError);
            }

            if (gdieExistente is null)
            {
                return Response.Failure(Error.GenericError("No se recibió información del usuario desde GDIE."));
            }

            if (gdieExistente.Codigo != 0)
            {
                return Response.Failure(Error.GenericError(
                    gdieExistente.MensajeNormalizado ?? "Error al consultar usuario en GDIE."));
            }

            var uidGDIE = !string.IsNullOrWhiteSpace(gdieExistente.IdIdentidades)
                ? gdieExistente.IdIdentidades.Trim()
                : usuario.IdRed.Trim();

            var rolesActuales = usuario.Roles?.ToList() ?? new List<UsuarioRol>();

            foreach (var rol in rolesActuales)
            {
                var gdieRol = ObtenerRolGDIE(rol.RolId);

                if (string.IsNullOrWhiteSpace(gdieRol))
                {
                    continue;
                }

                var cancelDto = new GestionarAccesoDTO
                {
                    Uid = uidGDIE,
                    Rol = gdieRol,
                };

                var (gdieResponse, gdieError) = await this.GDIEServiceConsumer.CancelarAccesoAsync(cancelDto);

                if (gdieError is not null)
                {
                    return Response.Failure(gdieError);
                }

                if (gdieResponse is null)
                {
                    return Response.Failure(Error.GenericError("No se recibió respuesta desde GDIE al cancelar el acceso."));
                }

                // -22 = Permiso no asignado. Para desactivar no debe bloquear,
                // porque el usuario local puede continuar desactivándose.
                if (gdieResponse.Codigo != 0 && gdieResponse.Codigo != -22)
                {
                    return Response.Failure(Error.GenericError(
                        gdieResponse.MensajeNormalizado ?? "Error al cancelar acceso en GDIE."));
                }
            }

            usuario.Activo = false;

            if (!string.IsNullOrWhiteSpace(dto.CodigoIncidencia))
            {
                usuario.CodigoIncidencia = dto.CodigoIncidencia;
            }

            foreach (var rol in rolesActuales)
            {
                this.UnitOfWork.UsuarioRolRepository.Remove(rol);
            }

            await this.UnitOfWork.SaveAsync(cancellationToken, logTecnicoComment: dto.Comentario);

            return Response.Success();
        }

        private static string ObtenerRolGDIE(int rolId) => rolId switch
        {
            (int)RolesEnum.Contratista => "transformacionsrccontratista",
            (int)RolesEnum.EstudioAuditor => "transformacionsrcestudioauditor",
            _ => string.Empty,
        };
    }
}