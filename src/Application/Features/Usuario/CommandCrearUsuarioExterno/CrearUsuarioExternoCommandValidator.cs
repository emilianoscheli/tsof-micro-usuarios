namespace UsuariosAPI.Application.Features.Usuario.CommandCrearUsuarioExterno
{
    using System.Text.RegularExpressions;
    using APICore.Enums;
    using APICore.Helpers;
    using FluentValidation;

    public class CrearUsuarioExternoCommandValidator : AbstractValidator<CrearUsuarioExternoCommand>
    {
        private static readonly string[] PaisesHabilitados = new[]
        {
            "ARGENTINA", "BOLIVIA", "BRASIL", "CHILE", "COLOMBIA", "ECUADOR", "PARAGUAY", "PERU", "URUGUAY", "VENEZUELA",
            "CANADA", "ESTADOS UNIDOS", "MEXICO", "REP DOMINICANA", "ALEMANIA", "ESPAÑA", "FRANCIA", "ITALIA", "INGLATERRA",
            "JAPON", "SINGAPUR", "OTRO",
        };

        public CrearUsuarioExternoCommandValidator()
        {
            this.RuleFor(x => x.dto)
                .NotNull()
                .WithMessage("El objeto DTO no puede ser nulo.")
                .DependentRules(() =>
                {
                    // Validar RolId (debe ser Contratista o Estudio Auditor)
                    this.RuleFor(x => x.dto.RolId)
                        .Must(rolId => rolId == (int)RolesEnum.Contratista || rolId == (int)RolesEnum.EstudioAuditor)
                        .WithMessage("El rol debe ser Contratista o Estudio Auditor");

                    // Validar ContratistaId
                    this.RuleFor(x => x.dto.ContratistaId)
                        .GreaterThan(0)
                        .WithMessage("El contratista es requerido");

                    // Validar Tipo de Documento
                    this.RuleFor(x => x.dto.TipoDocumentoId)
                        .GreaterThan(0)
                        .NotNull()
                        .WithMessage("El tipo de documento es requerido");

                    // Validar Número de Documento
                    this.RuleFor(x => x.dto.NumeroDocumento)
                        .NotEmpty()
                        .WithMessage("El número de documento es requerido")
                        .Length(2, 50)
                        .WithMessage("El número de documento debe tener entre 2 y 50 caracteres")
                        .Matches(@"^\d+$")
                        .WithMessage("El número de documento debe ser numérico");

                    // Validar CUIL - Campo obligatorio para todos los países
                    this.RuleFor(x => x.dto.CUIL)
                        .NotEmpty()
                        .WithMessage("El CUIL es requerido");

                    // Validaciones específicas para Argentina
                    this.When(x => x.dto.PaisId == 9, () =>
                    {
                        this.RuleFor(x => x.dto.CUIL)
                            .Length(11)
                            .WithMessage("El CUIL debe tener 11 dígitos numéricos (sin espacios, guiones o puntos)")
                            .Matches(@"^\d{11}$")
                            .WithMessage("El CUIL debe contener solo números")
                            .Must(ValidatorHelper.ValidarCuit)
                            .WithMessage("El CUIL ingresado no es válido.");
                    });

                    // Validación solo para Argentina: relación CUIL-DNI
                    this.When(x => x.dto.PaisId == 9, () =>
                    {
                        this.RuleFor(x => x.dto)
                            .Must(persona =>
                                !string.IsNullOrWhiteSpace(persona.CUIL) &&
                                !string.IsNullOrWhiteSpace(persona.NumeroDocumento) &&
                                persona.CUIL.Contains(persona.NumeroDocumento))
                            .WithMessage("El CUIL debe tener relación con el número de documento.");
                    });

                    // Validar Nombre
                    this.RuleFor(x => x.dto.Nombre)
                        .NotEmpty()
                        .WithMessage("El nombre es requerido")
                        .Length(3, 100)
                        .WithMessage("El nombre debe tener entre 3 y 100 caracteres")
                        .Matches(@"^[a-zA-ZáéíóúÁÉÍÓÚñÑüÜ\s]+$")
                        .WithMessage("El nombre solo puede contener letras, espacios y acentos");

                    // Validar Apellido
                    this.RuleFor(x => x.dto.Apellido)
                        .NotEmpty()
                        .WithMessage("El apellido es requerido")
                        .Length(3, 100)
                        .WithMessage("El apellido debe tener entre 3 y 100 caracteres")
                        .Matches(@"^[a-zA-ZáéíóúÁÉÍÓÚñÑüÜ\s]+$")
                        .WithMessage("El apellido solo puede contener letras, espacios y acentos");

                    // Validar País
                    this.RuleFor(x => x.dto.PaisId)
                        .NotNull()
                        .GreaterThan(0)
                        .WithMessage("El país es requerido");

                    // Validar Email
                    this.RuleFor(x => x.dto.EmailUsuario)
                        .NotEmpty()
                        .WithMessage("El email es requerido")
                        .Length(10, 80)
                        .WithMessage("El email debe tener entre 10 y 80 caracteres")
                        .EmailAddress()
                        .WithMessage("El email no tiene un formato válido")
                        .Must(email => !string.IsNullOrWhiteSpace(email) && email.Trim() == email)
                        .WithMessage("El email no debe contener espacios al inicio o al final");

                    // Validar Teléfono (opcional)
                    this.When(x => !string.IsNullOrWhiteSpace(x.dto.TelefonoParticular), () =>
                    {
                        this.RuleFor(x => x.dto.TelefonoParticular)
                            .Length(6, 15)
                            .WithMessage("El teléfono debe tener entre 6 y 15 caracteres")
                            .Matches(@"^\d+$")
                            .WithMessage("El teléfono debe ser numérico");
                    });

                    // Validar Código de Incidencia (opcional)
                    this.When(x => !string.IsNullOrWhiteSpace(x.dto.CodigoIncidencia), () =>
                    {
                        this.RuleFor(x => x.dto.CodigoIncidencia)
                            .MaximumLength(200)
                            .WithMessage("El código de incidencia no puede exceder 200 caracteres")
                            .Matches(@"^[a-zA-Z0-9\-]+$")
                            .WithMessage("El código de incidencia solo puede contener letras, números y guiones (sin espacios ni acentos)");
                    });

                    // Validación condicional: Si el rol es Estudio Auditor, se requiere Estudio y Sede
                    this.When(x => x.dto.RolId == (int)RolesEnum.EstudioAuditor, () =>
                    {
                        this.RuleFor(x => x.dto.EstudioAuditorId)
                            .NotNull()
                            .WithMessage("El estudio auditor es requerido para el rol Estudio Auditor")
                            .GreaterThan(0)
                            .WithMessage("El estudio auditor es requerido para el rol Estudio Auditor");

                        this.RuleFor(x => x.dto.SedeId)
                            .NotNull()
                            .WithMessage("La sede es requerida para el rol Estudio Auditor")
                            .GreaterThan(0)
                            .WithMessage("La sede es requerida para el rol Estudio Auditor");
                    });
                });
        }
    }
}
