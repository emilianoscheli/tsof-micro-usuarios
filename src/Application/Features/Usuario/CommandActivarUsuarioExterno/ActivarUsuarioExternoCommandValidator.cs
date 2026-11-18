namespace UsuariosAPI.Application.Features.Usuario.CommandActivarUsuarioExterno
{
    using APICore.Enums;
    using APICore.Responses;
    using FluentValidation;

    public class ActivarUsuarioExternoCommandValidator : AbstractValidator<ActivarUsuarioExternoCommand>
    {
        public ActivarUsuarioExternoCommandValidator()
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

                    // Validar RolId (debe ser Contratista o Estudio Auditor)
                    this.RuleFor(x => x.dto.RolId)
                        .Must(rolId => rolId == (int)RolesEnum.Contratista || rolId == (int)RolesEnum.EstudioAuditor)
                        .WithMessage("El rol debe ser Contratista o Estudio Auditor");

                    // Validar Código de Incidencia (opcional según HU05)
                    this.When(x => !string.IsNullOrWhiteSpace(x.dto.CodigoIncidencia), () =>
                    {
                        this.RuleFor(x => x.dto.CodigoIncidencia)
                            .MaximumLength(200)
                            .WithMessage("El código de incidencia no puede exceder 200 caracteres")
                            .Matches(@"^[a-zA-Z0-9\-]+$")
                            .WithMessage("El código de incidencia solo puede contener letras, números y guiones (sin espacios ni acentos)");
                    });

                    // Validación condicional: Si el rol es Estudio Auditor, se requiere Estudio y Sede
                    //this.When(x => x.dto.RolId == (int)RolesEnum.EstudioAuditor, () =>
                    //{
                    //    this.RuleFor(x => x.dto.EstudioAuditorId)
                    //        .NotNull()
                    //        .WithMessage("El estudio auditor es requerido para el rol Estudio Auditor")
                    //        .GreaterThan(0)
                    //        .WithMessage("El estudio auditor es requerido para el rol Estudio Auditor");

                    //    this.RuleFor(x => x.dto.SedeId)
                    //        .NotNull()
                    //        .WithMessage("La sede es requerida para el rol Estudio Auditor")
                    //        .GreaterThan(0)
                    //        .WithMessage("La sede es requerida para el rol Estudio Auditor");
                    //});
                });
        }
    }
}
