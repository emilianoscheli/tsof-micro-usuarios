namespace UsuariosAPI.Application.Features.Usuario.CommandActivarUsuarioExterno
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

    public class ActivarUsuarioExternoCommandHandler : ICommandHandler<ActivarUsuarioExternoCommand>
    {
        public ActivarUsuarioExternoCommandHandler(
            IUnitOfWork unitOfWork,
            IEstudioSedeServiceConsumer estudioSedeServiceConsumer,
            IGDIEServiceConsumer gdieServiceConsumer)
        {
            this.UnitOfWork = unitOfWork;
            this.EstudioSedeServiceConsumer = estudioSedeServiceConsumer;
            this.GDIEServiceConsumer = gdieServiceConsumer;
        }

        private IUnitOfWork UnitOfWork { get; }

        private IEstudioSedeServiceConsumer EstudioSedeServiceConsumer { get; }

        private IGDIEServiceConsumer GDIEServiceConsumer { get; }

        public async Task<IResponse> Handle(ActivarUsuarioExternoCommand request, CancellationToken cancellationToken)
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

            if (usuario.Activo)
            {
                return Response.Failure(UsuariosError.UsuarioExternoYaActivo);
            }

            // Validar que el rol sea Contratista o Estudio Auditor
            if (dto.RolId != (int)RolesEnum.Contratista && dto.RolId != (int)RolesEnum.EstudioAuditor)
            {
                return Response.Failure(UsuariosError.UsuarioExternoRolInvalido);
            }

            // Validar que el rol exista
            var rol = await this.UnitOfWork.RolRepository
                .GetByIdAsync(dto.RolId, cancellationToken);

            if (rol is null)
            {
                return Response.Failure(UsuariosError.EntidadNoEncontrada("Rol"));
            }

            // Verificar si el usuario existe en GDIE antes de intentar modificar
            var (gdieExistente, gdieExistenteError) = await this.GDIEServiceConsumer.ObtenerUsuarioPorUserIdAsync(usuario.IdRed ?? string.Empty);

            // Código -9: el usuario no existe en GDIE → se omite la gestión de accesos en GDIE
            // y se continúa con la activación local en SRC
            var usuarioExisteEnGDIE = gdieExistenteError is null && gdieExistente is not null && gdieExistente.Codigo == 0;

            if (gdieExistente is not null && gdieExistente.Codigo != 0 && gdieExistente.Codigo != -9)
            {
                return Response.Failure(Error.GenericError(gdieExistente.MensajeNormalizado ?? "Error al consultar usuario en GDIE"));
            }

            // Solicitar acceso en GDIE para el nuevo rol (solo si el usuario existe en GDIE)
            if (usuarioExisteEnGDIE)
            {
                var gdieRol = ObtenerRolGDIE(dto.RolId);
                if (!string.IsNullOrEmpty(gdieRol))
                {
                    var solicitarDto = new GestionarAccesoDTO { Uid = gdieExistente!.IdIdentidades!, Rol = gdieRol };
                    var (gdieResponse, gdieError) = await this.GDIEServiceConsumer.SolicitarAccesoAsync(solicitarDto);

                    // -24 = "Permiso asignado previamente" → no es un error real
                    if (gdieError is not null || gdieResponse is null || (gdieResponse.Codigo != 0 && gdieResponse.Codigo != -24))
                    {
                        return Response.Failure(Error.GenericError(gdieResponse?.MensajeNormalizado ?? "Error al solicitar acceso en GDIE"));
                    }
                }
            }

            // Activar usuario
            usuario.Activo = true;

            if (!string.IsNullOrEmpty(dto.CodigoIncidencia))
            {
                usuario.CodigoIncidencia = dto.CodigoIncidencia;
            }

            // Eliminar todos los roles existentes (usuarios externos solo pueden tener un rol)
            var rolesActuales = usuario.Roles!.ToList();
            if (rolesActuales.Any())
            {
                foreach (var rolActual in rolesActuales)
                {
                    this.UnitOfWork.UsuarioRolRepository.Remove(rolActual);
                }
            }

            // Asignar el nuevo rol
            var nuevoUsuarioRol = new UsuarioRol
            {
                UsuarioSRCId = usuario.Id,
                RolId = dto.RolId,
            };

            await this.UnitOfWork.UsuarioRolRepository.AddAsync(nuevoUsuarioRol, cancellationToken);

            await this.UnitOfWork.SaveAsync(cancellationToken);

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
