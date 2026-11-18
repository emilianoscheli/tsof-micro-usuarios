namespace UsuariosAPI.Application.Features.Usuario.CommandCrearUsuarioExterno
{
    using System.Threading;
    using System.Threading.Tasks;
    using APICore.Enums;
    using APICore.Interfaces.Responses;
    using APICore.Responses;
    using IntegracionesAPI.SDK.Contracts.DTOs.GDIE;
    using Microsoft.EntityFrameworkCore;
    using UsuariosAPI.Application.Abstractions.Messaging;
    using UsuariosAPI.Application.Interfaces;
    using UsuariosAPI.Domain.Abstractions;
    using UsuariosAPI.Domain.Abstractions.Errors;
    using UsuariosAPI.Domain.Usuario;
    using UsuariosAPI.Domain.UsuarioRol;

    public class CrearUsuarioExternoCommandHandler : ICommandHandler<CrearUsuarioExternoCommand>
    {
        private static readonly HashSet<string> PaisesPermitidosGDIE = new (StringComparer.OrdinalIgnoreCase)
        {
            "ARGENTINA", "BOLIVIA", "BRASIL", "CHILE", "COLOMBIA", "ECUADOR", "PARAGUAY", "PERU",
            "URUGUAY", "VENEZUELA", "CANADA", "ESTADOS UNIDOS", "MEXICO", "REP DOMINICANA",
            "ALEMANIA", "ESPAÑA", "FRANCIA", "ITALIA", "INGLATERRA", "JAPON", "SINGAPUR", "OTRO",
        };

        public CrearUsuarioExternoCommandHandler(
           IUnitOfWork unitOfWork,
           IContratistaServiceConsumer contratistaServiceConsumer,
           IEstudioSedeServiceConsumer estudioSedeServiceConsumer,
           ITipoDocumentoServiceConsumer tipoDocumentoServiceConsumer,
           IUbicacionServiceConsumer ubicacionServiceConsumer,
           IGDIEServiceConsumer gdieServiceConsumer)
        {
            this.UnitOfWork = unitOfWork;
            this.ContratistaServiceConsumer = contratistaServiceConsumer;
            this.EstudioSedeServiceConsumer = estudioSedeServiceConsumer;
            this.TipoDocumentoServiceConsumer = tipoDocumentoServiceConsumer;
            this.UbicacionServiceConsumer = ubicacionServiceConsumer;
            this.GDIEServiceConsumer = gdieServiceConsumer;
        }

        private IUnitOfWork UnitOfWork { get; }

        private IContratistaServiceConsumer ContratistaServiceConsumer { get; }

        private IEstudioSedeServiceConsumer EstudioSedeServiceConsumer { get; }

        private ITipoDocumentoServiceConsumer TipoDocumentoServiceConsumer { get; }

        private IUbicacionServiceConsumer UbicacionServiceConsumer { get; }

        private IGDIEServiceConsumer GDIEServiceConsumer { get; }

        public async Task<IResponse> Handle(CrearUsuarioExternoCommand request, CancellationToken cancellationToken)
        {
            var dto = request.dto;

            // Validar que el rol sea Contratista o Estudio Auditor
            if (dto.RolId != (int)RolesEnum.Contratista && dto.RolId != (int)RolesEnum.EstudioAuditor)
            {
                return Response.Failure(UsuariosError.UsuarioExternoRolInvalido);
            }

            // Validar tipo de documento
            var (tipoDocumento, errorTipoDocumento) = await this.TipoDocumentoServiceConsumer.GetTipoDocumentoById(dto.TipoDocumentoId);
            if (errorTipoDocumento is not null)
            {
                return Response.Failure(errorTipoDocumento);
            }

            // Validar país y obtener nombre para GDIE
            var (pais, errorPais) = await this.UbicacionServiceConsumer.GetPaisesByIds([dto.PaisId]);
            if (errorPais is not null || !pais.Any())
            {
                return Response.Failure(UsuariosError.EntidadNoEncontrada("Pais"));
            }

            // Validar contratista
            var (contratista, errorContratista) = await this.ContratistaServiceConsumer.ValidateContratista(dto.ContratistaId);
            if (errorContratista is not null)
            {
                return Response.Failure(errorContratista);
            }

            // Si el rol es Estudio Auditor, validar estudio y sede
            if (dto.RolId == (int)RolesEnum.EstudioAuditor)
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
            }

            // Validar que el rol exista
            var rol = await this.UnitOfWork.RolRepository
                .GetByIdAsync(dto.RolId, cancellationToken);

            if (rol is null)
            {
                return Response.Failure(UsuariosError.EntidadNoEncontrada("Rol"));
            }

            // Validar que no exista usuario externo activo con mismo email
            var usuarioExistentePorEmail = await this.UnitOfWork.UsuarioSRCRepository
                .AsQueryable()
                .Where(u => u.EsExterno && u.Activo && u.CorreoElectronico == dto.EmailUsuario)
                .FirstOrDefaultAsync(cancellationToken);

            if (usuarioExistentePorEmail is not null)
            {
                return Response.Failure(UsuariosError.DatoExistente($"Email de usuario: {dto.EmailUsuario}"));
            }

            // Mapear nombre de país al formato permitido por GDIE
            var nombrePaisGDIE = ObtenerNombrePaisGDIE(pais!.FirstOrDefault()!.ISOName);

            // Formatear CUIL para GDIE (XX-XXXXXXXX-X) solo si es Argentina
            var esArgentina = pais.FirstOrDefault()!.EsArgentina ?? false;
            var cuilGDIE = esArgentina ? FormatearCuil(dto.CUIL) : dto.CUIL;

            // TipoDocumento para GDIE: si Argentina → "C.U.I.L.", si no → nombre del tipo de documento
            var tipoDocumentoGDIE = esArgentina ? "C.U.I.L." : (tipoDocumento?.Nombre ?? string.Empty);

            // Llamar al web service de GDIE para registrar el usuario
            var gdieDto = new CrearUsuarioGDIEDTO
            {
                Nombre = dto.Nombre,
                Apellido = dto.Apellido,
                Email = dto.EmailUsuario,
                Pais = nombrePaisGDIE,
                Telefono = dto.TelefonoParticular ?? string.Empty,
                Cuit = cuilGDIE,
                Empresa = contratista?.RazonSocial ?? string.Empty,
                Rol = ObtenerRolGDIE(dto.RolId),
                TipoDocumento = tipoDocumentoGDIE,
                NumeroDocumento = cuilGDIE,
            };

            var (gdieResponse, gdieError) = await this.GDIEServiceConsumer.RegistrarUsuarioAsync(gdieDto);

            // Determinar el IdRed resultante
            string idRed;

            if (gdieError is null && gdieResponse is not null && gdieResponse.Codigo == 0)
            {
                // Creación exitosa en GDIE
                idRed = gdieResponse.IdIdentidades ?? $"EP{dto.NumeroDocumento}";
            }
            else if (gdieResponse?.Codigo == -16)
            {
                // El email ya existe en GDIE como EP: crear localmente sin error (HU02)
                idRed = gdieResponse.IdIdentidades ?? $"EP{dto.NumeroDocumento}";
            }
            else
            {
                // Otro error de GDIE: retornar el mensaje del servicio
                return Response.Failure(Error.GenericError(gdieResponse?.MensajeNormalizado ?? "Error al registrar el usuario en GDIE"));
            }

            // Crear el usuario externo en SRC
            var usuario = new UsuarioSRC
            {
                NombreCompleto = $"{dto.Nombre} {dto.Apellido}",
                IdRed = idRed,
                CorreoElectronico = dto.EmailUsuario,
                EsExterno = true,
                TipoDocumentoFkId = dto.TipoDocumentoId,
                NumeroDocumento = dto.NumeroDocumento,
                Cuil = dto.CUIL,
                PaisFkId = dto.PaisId,
                TelefonoParticular = dto.TelefonoParticular,
                CodigoIncidencia = dto.CodigoIncidencia,
                ContratistaFkId = dto.ContratistaId,
                EstudioAuditorFkId = dto.EstudioAuditorId,
                SedeFkId = dto.SedeId,
                Activo = true,
            };

            await this.UnitOfWork.UsuarioSRCRepository.AddAsync(usuario, cancellationToken);
            await this.UnitOfWork.SaveAsync(cancellationToken);

            // Agregar rol después de guardar para que el usuario tenga un Id
            var usuarioRol = new UsuarioRol
            {
                UsuarioSRCId = usuario.Id,
                RolId = dto.RolId,
            };

            await this.UnitOfWork.UsuarioRolRepository.AddAsync(usuarioRol, cancellationToken);
            await this.UnitOfWork.SaveAsync(cancellationToken);

            return Response.Success();
        }

        private static string ObtenerNombrePaisGDIE(string? nombrePais)
        {
            if (string.IsNullOrWhiteSpace(nombrePais))
            {
                return "OTRO";
            }

            var nombreUpper = nombrePais.Trim().ToUpperInvariant();

            return PaisesPermitidosGDIE.Contains(nombreUpper) ? nombreUpper : "OTRO";
        }

        private static string FormatearCuil(string cuil)
        {
            if (string.IsNullOrWhiteSpace(cuil) || cuil.Length != 11)
            {
                return cuil;
            }

            return $"{cuil[..2]}-{cuil[2..10]}-{cuil[10]}";
        }

        private static List<string> ObtenerRolGDIE(int rolId)
        {
            var lista = new List<string>();

            switch (rolId)
            {
                case (int)RolesEnum.Contratista:
                        lista.Add("transformacionsrccontratista");
                        break;
                case (int)RolesEnum.EstudioAuditor:
                        lista.Add("transformacionsrcestudioauditor");
                        break;
                default:
                        break;
            }

            return lista;
        }
    }
}
