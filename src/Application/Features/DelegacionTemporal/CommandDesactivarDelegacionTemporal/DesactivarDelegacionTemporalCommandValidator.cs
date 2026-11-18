namespace UsuariosAPI.Application.Features.DelegacionTemporal.CommandDesactivarDelegacionTemporal
{
    using APICore.Responses;
    using FluentValidation;

    public class DesactivarDelegacionTemporalCommandValidator : AbstractValidator<DesactivarDelegacionTemporalCommand>
    {
        public DesactivarDelegacionTemporalCommandValidator()
        {
            this.RuleFor(x => x.dto.Id).GreaterThan(0);

            this.RuleFor(x => x.dto.Motivo)
                .NotNull().NotEmpty().WithMessage(Error.CampoObligatorio.description)
                .Length(10, 250).WithMessage(Error.InvalidLength("10", "250").description)
                .Matches(@"^[A-Za-z0-9!@#$%^&*()_+={}\[\]:;""'<>,.?\/\\|`~\- ]*$$").WithMessage(Error.InvalidCharacters.description);
        }
    }
}
