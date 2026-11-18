namespace UsuariosAPI.Application.Features.Usuario.CommandCambiarRolUsuarioExterno
{
    using APICore.Enums;
    using APICore.Responses;
    using FluentValidation;

    public class CambiarRolUsuarioExternoCommandValidator : AbstractValidator<CambiarRolUsuarioExternoCommand>
    {
        public CambiarRolUsuarioExternoCommandValidator()
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

                    // Validar NuevoRolId (debe ser Contratista o Estudio Auditor)
                    this.RuleFor(x => x.dto.NuevoRolId)
                        .Must(rolId => rolId == (int)RolesEnum.Contratista || rolId == (int)RolesEnum.EstudioAuditor)
                        .WithMessage("El nuevo rol debe ser Contratista o Estudio Auditor");

                    // Validación condicional: Si el nuevo rol es Estudio Auditor, se requiere Estudio y Sede
                    this.When(x => x.dto.NuevoRolId == (int)RolesEnum.EstudioAuditor, () =>
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
