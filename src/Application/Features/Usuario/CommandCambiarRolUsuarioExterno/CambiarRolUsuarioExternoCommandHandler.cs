namespace UsuariosAPI.Application.Features.Usuario.CommandCambiarRolUsuarioExterno
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

    public class CambiarRolUsuarioExternoCommandHandler : ICommandHandler<CambiarRolUsuarioExternoCommand>
    {
        public CambiarRolUsuarioExternoCommandHandler(
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

        public async Task<IResponse> Handle(CambiarRolUsuarioExternoCommand request, CancellationToken cancellationToken)
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

            // Validar que el nuevo rol sea Contratista o Estudio Auditor
            if (dto.NuevoRolId != (int)RolesEnum.Contratista && dto.NuevoRolId != (int)RolesEnum.EstudioAuditor)
            {
                return Response.Failure(UsuariosError.UsuarioExternoRolInvalido);
            }

            // Validar que el rol exista
            var nuevoRol = await this.UnitOfWork.RolRepository
                .GetByIdAsync(dto.NuevoRolId, cancellationToken);

            if (nuevoRol is null)
            {
                return Response.Failure(UsuariosError.EntidadNoEncontrada("Rol"));
            }

            // Si el nuevo rol es Estudio Auditor, validar estudio y sede
            if (dto.NuevoRolId == (int)RolesEnum.EstudioAuditor)
            {
                if (!dto.EstudioAuditorId.HasValue || !dto.SedeId.HasValue)
                {
                    return Response.Failure(Error.GenericError("Para el rol Estudio Auditor se requiere estudio y sede"));
                }

                var (estudio, errorEstudio) = await this.EstudioSedeServiceConsumer.ValidateEstudioAuditor(dto.EstudioAuditorId.Value);
                if (errorEstudio is not null)
                {
                    return Response.Failure(errorEstudio);
                }

                var (sede, errorSede) = await this.EstudioSedeServiceConsumer.ValidateSede(dto.SedeId.Value);
                if (errorSede is not null)
                {
                    return Response.Failure(errorSede);
                }

                usuario.EstudioAuditorFkId = dto.EstudioAuditorId;
                usuario.SedeFkId = dto.SedeId;
            }
            else
            {
                // Si cambia a Contratista, eliminar estudio y sede
                usuario.EstudioAuditorFkId = null;
                usuario.SedeFkId = null;
            }

            // Verificar si el usuario existe en GDIE antes de intentar modificar
            var (gdieExistente, gdieExistenteError) = await this.GDIEServiceConsumer.ObtenerUsuarioPorEmailAsync(usuario.CorreoElectronico ?? string.Empty);

            // Código -9: el email no existe en GDIE → el usuario no está registrado en GDIE,
            // se omite la gestión de accesos en GDIE y se continúa con el cambio de rol local en SRC
            var usuarioExisteEnGDIE = gdieExistenteError is null && gdieExistente is not null && gdieExistente.Codigo == 0;

            if (usuarioExisteEnGDIE)
            {
                // Cancelar acceso del rol actual en GDIE
                var rolActualId = usuario.Roles!.FirstOrDefault()?.RolId;
                if (rolActualId.HasValue)
                {
                    var gdieRolActual = ObtenerRolGDIE(rolActualId.Value);
                    if (!string.IsNullOrEmpty(gdieRolActual))
                    {
                        var cancelDto = new GestionarAccesoDTO { Uid = gdieExistente!.IdIdentidades!, Rol = gdieRolActual };
                        var (cancelResponse, cancelError) = await this.GDIEServiceConsumer.CancelarAccesoAsync(cancelDto);

                        // -22 = "Permiso no asignado" → no es un error real
                        if (cancelError is not null || cancelResponse is null || (cancelResponse.Codigo != 0 && cancelResponse.Codigo != -22))
                        {
                            return Response.Failure(Error.GenericError(cancelResponse?.MensajeNormalizado ?? "Error al cancelar acceso en GDIE"));
                        }
                    }
                }

                // Solicitar acceso del nuevo rol en GDIE
                var gdieRolNuevo = ObtenerRolGDIE(dto.NuevoRolId);
                if (!string.IsNullOrEmpty(gdieRolNuevo))
                {
                    var solicitarDto = new GestionarAccesoDTO { Uid = gdieExistente!.IdIdentidades!, Rol = gdieRolNuevo };
                    var (solicitarResponse, solicitarError) = await this.GDIEServiceConsumer.SolicitarAccesoAsync(solicitarDto);

                    // -24 = "Permiso asignado previamente" → no es un error real
                    if (solicitarError is not null || solicitarResponse is null || (solicitarResponse.Codigo != 0 && solicitarResponse.Codigo != -24))
                    {
                        return Response.Failure(Error.GenericError(solicitarResponse?.MensajeNormalizado ?? "Error al solicitar acceso en GDIE"));
                    }
                }
            }

            // Eliminar rol actual y agregar el nuevo
            var rolesActuales = usuario.Roles!.ToList();
            foreach (var rolActual in rolesActuales)
            {
                this.UnitOfWork.UsuarioRolRepository.Remove(rolActual);
            }

            // Agregar nuevo rol
            var nuevoUsuarioRol = new UsuarioRol
            {
                UsuarioSRCId = usuario.Id,
                RolId = dto.NuevoRolId,
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
