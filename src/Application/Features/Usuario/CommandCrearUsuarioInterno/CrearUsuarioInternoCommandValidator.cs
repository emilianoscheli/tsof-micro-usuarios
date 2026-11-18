namespace UsuariosAPI.Application.Features.Usuario.CommandCrearUsuarioInterno
{
    using APICore.Responses;
    using FluentValidation;

    public class CrearUsuarioInternoCommandValidator : AbstractValidator<CrearUsuarioInternoCommand>
    {
        public CrearUsuarioInternoCommandValidator()
        {
            this.RuleFor(x => x.dto)
                .NotNull()
                .WithMessage("El objeto DTO no puede ser nulo.")
                .DependentRules(() =>
                {
                    this.RuleFor(x => x.dto.IdRed)
                        .NotEmpty()
                        .WithMessage(Error.CampoObligatorio.description);
                });
        }
    }
}
