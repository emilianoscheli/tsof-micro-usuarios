namespace UsuariosAPI.Application.Features.Usuario.CommandAgregarRoles
{
    using APICore.Responses;
    using FluentValidation;

    public class AgregarRolesCommandValidator : AbstractValidator<AgregarRolesCommand>
    {
        public AgregarRolesCommandValidator()
        {
            this.RuleFor(x => x.dto.UsuariosRoles).NotNull().NotEmpty().WithMessage(Error.CampoObligatorio.description);
            this.RuleForEach(x => x.dto.UsuariosRoles).ChildRules(item =>
            {
                item.RuleFor(i => i.UsuarioId)
                    .GreaterThan(0).WithMessage(Error.CampoObligatorio.description);

                item.RuleFor(i => i.RolId)
                    .GreaterThan(0).WithMessage(Error.CampoObligatorio.description);
            });
        }
    }
}
