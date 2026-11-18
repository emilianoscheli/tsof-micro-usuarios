namespace UsuariosAPI.Application.Features.Usuario.CommandEditarDatosPersonalesUsuarioExterno
{
    using APICore.Responses;
    using FluentValidation;

    public class EditarDatosPersonalesUsuarioExternoCommandValidator : AbstractValidator<EditarDatosPersonalesUsuarioExternoCommand>
    {
        public EditarDatosPersonalesUsuarioExternoCommandValidator()
        {
            this.RuleFor(x => x.dto)
                .NotNull()
                .WithMessage("El objeto DTO no puede ser nulo.")
                .DependentRules(() =>
                {
                    // Validar UsuarioId (obligatorio)
                    this.RuleFor(x => x.dto.UsuarioId)
                        .GreaterThan(0)
                        .WithMessage(Error.CampoObligatorio.description);

                    // Validar ContratistaId (opcional, pero si se proporciona debe ser válido)
                    this.When(x => x.dto.ContratistaId.HasValue, () =>
                    {
                        this.RuleFor(x => x.dto.ContratistaId)
                            .GreaterThan(0)
                            .WithMessage("El contratista debe ser un valor válido");
                    });

                    // Validar Nombre (opcional, pero si se proporciona debe ser válido)
                    this.When(x => !string.IsNullOrWhiteSpace(x.dto.Nombre), () =>
                    {
                        this.RuleFor(x => x.dto.Nombre)
                            .Length(3, 100)
                            .WithMessage("El nombre debe tener entre 3 y 100 caracteres")
                            .Matches(@"^[a-zA-ZáéíóúÁÉÍÓÚñÑüÜ\s]+$")
                            .WithMessage("El nombre solo puede contener letras, espacios y acentos");
                    });

                    // Validar Apellido (opcional, pero si se proporciona debe ser válido)
                    this.When(x => !string.IsNullOrWhiteSpace(x.dto.Apellido), () =>
                    {
                        this.RuleFor(x => x.dto.Apellido)
                            .Length(3, 100)
                            .WithMessage("El apellido debe tener entre 3 y 100 caracteres")
                            .Matches(@"^[a-zA-ZáéíóúÁÉÍÓÚñÑüÜ\s]+$")
                            .WithMessage("El apellido solo puede contener letras, espacios y acentos");
                    });

                    // Validar Pais (opcional, pero si se proporciona debe ser válido)
                    this.When(x => x.dto.Pais.HasValue, () =>
                    {
                        this.RuleFor(x => x.dto.Pais)
                            .GreaterThan(0)
                            .WithMessage("El país debe ser un valor válido");
                    });

                    // Validar Email (opcional, pero si se proporciona debe ser válido)
                    this.When(x => !string.IsNullOrWhiteSpace(x.dto.EmailEmpresa), () =>
                    {
                        this.RuleFor(x => x.dto.EmailEmpresa)
                            .Length(10, 80)
                            .WithMessage("El email debe tener entre 10 y 80 caracteres")
                            .EmailAddress()
                            .WithMessage("El email no tiene un formato válido")
                            .Must(email => !string.IsNullOrWhiteSpace(email) && email.Trim() == email)
                            .WithMessage("El email no debe contener espacios al inicio o al final");
                    });

                    // Validar Teléfono (opcional, pero si se proporciona debe ser válido)
                    this.When(x => !string.IsNullOrWhiteSpace(x.dto.TelefonoParticular), () =>
                    {
                        this.RuleFor(x => x.dto.TelefonoParticular)
                            .Length(6, 15)
                            .WithMessage("El teléfono debe tener entre 6 y 15 caracteres")
                            .Matches(@"^\d+$")
                            .WithMessage("El teléfono debe ser numérico");
                    });

                    // Validar EstudioAuditorId (opcional, pero si se proporciona debe ser válido)
                    this.When(x => x.dto.EstudioAuditorId.HasValue, () =>
                    {
                        this.RuleFor(x => x.dto.EstudioAuditorId)
                            .GreaterThan(0)
                            .WithMessage("El estudio auditor debe ser un valor válido");
                    });

                    // Validar SedeId (opcional, pero si se proporciona debe ser válido)
                    this.When(x => x.dto.SedeId.HasValue, () =>
                    {
                        this.RuleFor(x => x.dto.SedeId)
                            .GreaterThan(0)
                            .WithMessage("La sede debe ser un valor válido");
                    });
                });
        }
    }
}
