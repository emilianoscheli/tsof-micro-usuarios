namespace UsuariosAPI.Application.Features.Usuario.CommandEditarDatosPersonalesUsuarioExterno
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

    public class EditarDatosPersonalesUsuarioExternoCommandHandler : ICommandHandler<EditarDatosPersonalesUsuarioExternoCommand>
    {
        private static readonly HashSet<string> PaisesPermitidosGDIE = new (StringComparer.OrdinalIgnoreCase)
        {
            "ARGENTINA", "BOLIVIA", "BRASIL", "CHILE", "COLOMBIA", "ECUADOR", "PARAGUAY", "PERU",
            "URUGUAY", "VENEZUELA", "CANADA", "ESTADOS UNIDOS", "MEXICO", "REP DOMINICANA",
            "ALEMANIA", "ESPAÑA", "FRANCIA", "ITALIA", "INGLATERRA", "JAPON", "SINGAPUR", "OTRO",
        };

        public EditarDatosPersonalesUsuarioExternoCommandHandler(
           IUnitOfWork unitOfWork,
           IContratistaServiceConsumer contratistaServiceConsumer,
           IEstudioSedeServiceConsumer estudioSedeServiceConsumer,
           IUbicacionServiceConsumer ubicacionServiceConsumer,
           IGDIEServiceConsumer gdieServiceConsumer)
        {
            this.UnitOfWork = unitOfWork;
            this.ContratistaServiceConsumer = contratistaServiceConsumer;
            this.EstudioSedeServiceConsumer = estudioSedeServiceConsumer;
            this.UbicacionServiceConsumer = ubicacionServiceConsumer;
            this.GDIEServiceConsumer = gdieServiceConsumer;
        }

        private IUnitOfWork UnitOfWork { get; }

        private IContratistaServiceConsumer ContratistaServiceConsumer { get; }

        private IEstudioSedeServiceConsumer EstudioSedeServiceConsumer { get; }

        private IUbicacionServiceConsumer UbicacionServiceConsumer { get; }

        private IGDIEServiceConsumer GDIEServiceConsumer { get; }

        public async Task<IResponse> Handle(EditarDatosPersonalesUsuarioExternoCommand request, CancellationToken cancellationToken)
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

            // Validar país si se proporciona y obtener nombre para GDIE
            string? nombrePaisGDIE = null;
            if (dto.Pais.HasValue)
            {
                var (pais, errorPais) = await this.UbicacionServiceConsumer.GetPaisesByIds([dto.Pais.Value]);
                if (errorPais is not null || !pais.Any())
                {
                    return Response.Failure(UsuariosError.EntidadNoEncontrada("Pais"));
                }

                nombrePaisGDIE = ObtenerNombrePaisGDIE(pais!.FirstOrDefault()!.ISOName);
            }

            // Validar contratista si se proporciona
            string? empresaGDIE = null;
            if (dto.ContratistaId.HasValue)
            {
                var (contratista, errorContratista) = await this.ContratistaServiceConsumer.ValidateContratista(dto.ContratistaId.Value);
                if (errorContratista is not null)
                {
                    return Response.Failure(errorContratista);
                }

                empresaGDIE = contratista?.RazonSocial;
            }

            // Verificar si el usuario tiene rol Estudio Auditor
            var tieneRolEstudioAuditor = usuario.Roles!.Any(r => r.RolId == (int)RolesEnum.EstudioAuditor);

            // Si tiene rol Estudio Auditor o se están enviando estudio/sede, validar
            if (tieneRolEstudioAuditor || dto.EstudioAuditorId.HasValue || dto.SedeId.HasValue)
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

            // Verificar si el usuario existe en GDIE antes de intentar modificar
            var (gdieExistente, gdieExistenteError) = await this.GDIEServiceConsumer.ObtenerUsuarioPorEmailAsync(usuario.CorreoElectronico ?? string.Empty);

            // Código -9: el email no existe en GDIE → el usuario no está registrado en GDIE,
            // se omite la modificación en GDIE y se continúa con la actualización local en SRC
            if (gdieExistenteError is null && gdieExistente is not null && gdieExistente.Codigo == 0)
            {
                // El usuario existe en GDIE → llamar al WS para modificar los datos
                var gdieDto = new ModificarUsuarioGDIEDTO
                {
                    IdIdentidades = gdieExistente.IdIdentidades!,
                    Nombre = !string.IsNullOrWhiteSpace(dto.Nombre) ? dto.Nombre : gdieExistente.Usuario!.Nombre!,
                    Apellido = !string.IsNullOrWhiteSpace(dto.Apellido) ? dto.Apellido : gdieExistente.Usuario!.Apellido!,
                    Email = !string.IsNullOrWhiteSpace(dto.EmailEmpresa) ? dto.EmailEmpresa : gdieExistente.Usuario!.Mail!,
                    Pais = nombrePaisGDIE ?? gdieExistente.Usuario!.Pais!,
                    Telefono = !string.IsNullOrWhiteSpace(dto.TelefonoParticular) ? dto.TelefonoParticular : gdieExistente.Usuario!.Telefono!,
                    Cuit = gdieExistente.Usuario!.Cuit!, // No se modifica el CUIT porque no se recibe en el DTO de SRC
                    Empresa = empresaGDIE ?? gdieExistente.Usuario!.Empresa!,
                    Rol = [gdieExistente.Usuario!.Permisos], // No se modifica el rol porque no se recibe en el DTO de SRC
                    TipoDocumento = gdieExistente.Usuario!.TipoDocumento, // No se modifica el tipo de documento porque no se recibe en el DTO de SRC
                    NumeroDocumento = gdieExistente.Usuario!.NumeroDocumento, // No se modifica el número de documento porque no se recibe en el DTO de SRC
                };

                var (gdieResponse, gdieError) = await this.GDIEServiceConsumer.ModificarDatosPersonalesAsync(gdieDto);

                if (gdieError is not null || gdieResponse is null || (gdieResponse.Codigo != 0 && gdieResponse.Codigo != -21))
                {
                    // -21 significa "Los datos enviados coinciden con los del usuario" → no es un error real
                    return Response.Failure(Error.GenericError(gdieResponse?.MensajeNormalizado ?? "Error al modificar el usuario en GDIE"));
                }
            }

            // Actualizar solo los campos que se proporcionan en SRC
            if (!string.IsNullOrWhiteSpace(dto.Nombre) && !string.IsNullOrWhiteSpace(dto.Apellido))
            {
                usuario.NombreCompleto = $"{dto.Nombre} {dto.Apellido}";
            }
            else if (!string.IsNullOrWhiteSpace(dto.Nombre) || !string.IsNullOrWhiteSpace(dto.Apellido))
            {
                var nombreParts = usuario.NombreCompleto?.Split(' ', 2) ?? new[] { string.Empty, string.Empty };
                var nombre = !string.IsNullOrWhiteSpace(dto.Nombre) ? dto.Nombre : nombreParts.Length > 0 ? nombreParts[0] : string.Empty;
                var apellido = !string.IsNullOrWhiteSpace(dto.Apellido) ? dto.Apellido : nombreParts.Length > 1 ? nombreParts[1] : string.Empty;
                usuario.NombreCompleto = $"{nombre} {apellido}".Trim();
            }

            if (dto.Pais.HasValue)
            {
                usuario.PaisFkId = dto.Pais.Value;
            }

            if (!string.IsNullOrWhiteSpace(dto.EmailEmpresa))
            {
                usuario.CorreoElectronico = dto.EmailEmpresa;
            }

            if (dto.TelefonoParticular is not null)
            {
                usuario.TelefonoParticular = dto.TelefonoParticular;
            }

            if (dto.ContratistaId.HasValue)
            {
                usuario.ContratistaFkId = dto.ContratistaId.Value;
            }

            if (dto.EstudioAuditorId.HasValue)
            {
                usuario.EstudioAuditorFkId = dto.EstudioAuditorId.Value;
            }

            if (dto.SedeId.HasValue)
            {
                usuario.SedeFkId = dto.SedeId.Value;
            }

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
    }
}
