namespace UsuariosAPI.Application.Features.Usuario.CommandDesactivarUsuarioExterno
{
    using APICore.Responses;
    using FluentValidation;

    public class DesactivarUsuarioExternoCommandValidator : AbstractValidator<DesactivarUsuarioExternoCommand>
    {
        public DesactivarUsuarioExternoCommandValidator()
        {
            this.RuleFor(x => x.dto)
                .NotNull()
                .WithMessage("El objeto DTO no puede ser nulo.")
                .DependentRules(() =>
                {
                    // Validar UsuarioId
                    this.RuleFor(x => x.dto.UsuarioId)
                        .GreaterThan(0)
                        .WithMessage(Error.CampoObligatorio.description);

                    // Validar Código de Incidencia (opcional según HU05)
                    this.When(x => !string.IsNullOrWhiteSpace(x.dto.CodigoIncidencia), () =>
                    {
                        this.RuleFor(x => x.dto.CodigoIncidencia)
                            .MaximumLength(200)
                            .WithMessage("El código de incidencia no puede exceder 200 caracteres")
                            .Matches(@"^[a-zA-Z0-9\-]+$")
                            .WithMessage("El código de incidencia solo puede contener letras, números y guiones (sin espacios ni acentos)");
                    });

                    // Validar Comentario
                    this.RuleFor(x => x.dto.Comentario)
                        .NotEmpty()
                        .NotNull()
                        .WithMessage(Error.CampoObligatorio.description);

                    this.RuleFor(x => x.dto.Comentario)
                        .MaximumLength(500)
                        .WithMessage("El comentario no puede exceder 500 caracteres");
                });
        }
    }
}
