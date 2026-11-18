namespace UsuariosAPI.Application.Features.Usuario.CommandModificarRolesSociedades
{
    using APICore.Responses;
    using FluentValidation;

    public class ModificarRolesSociedadesCommandValidator : AbstractValidator<ModificarRolesSociedadesCommand>
    {
        public ModificarRolesSociedadesCommandValidator()
        {
            this.RuleFor(x => x.dto)
                .NotNull()
                .WithMessage("El objeto DTO no puede ser nulo.")
                .DependentRules(() =>
                {
                    // Validar UsuarioSRCId
                    this.RuleFor(x => x.dto.UsuarioSRCId)
                        .GreaterThan(0)
                        .WithMessage(Error.CampoObligatorio.description);

                    // Ver si habria que validar las listas
                });
        }
    }
}
