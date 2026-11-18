namespace UsuariosAPI.Application.Features.DelegacionTemporal.CommandCreateDelegacionTemporal
{
    using APICore.Responses;
    using FluentValidation;

    public class CreateDelegacionTemporalCommandValidator : AbstractValidator<CreateDelegacionTemporalCommand>
    {
        public CreateDelegacionTemporalCommandValidator()
        {
            this.RuleFor(x => x.dto.FechaDesde)
               .NotNull().WithMessage("Campo obligatorio")
               .Must(date => date >= DateOnly.FromDateTime(DateTime.Today))
               .WithMessage("No se permite una fecha pasada");

            this.RuleFor(x => x.dto.FechaHasta)
                .NotNull().WithMessage("Campo obligatorio")
                .Must((cmd, fechaHasta) => fechaHasta >= DateOnly.FromDateTime(DateTime.Today))
                .WithMessage("No se permite una fecha pasada")
                .Must((cmd, fechaHasta) => fechaHasta >= cmd.dto.FechaDesde)
                .WithMessage("Debe ser igual o posterior a la fecha desde")
                .Must((cmd, fechaHasta) => fechaHasta <= cmd.dto.FechaDesde.AddDays(cmd.dto.TiempoLimite))
                .WithMessage("Excede el límite de días permitidos para el motivo seleccionado");

            this.RuleFor(x => x.dto.MotivoId).GreaterThan(0).WithMessage(Error.CampoObligatorio.description);

            this.When(x => !string.IsNullOrEmpty(x.dto.Observacion), () =>
            {
                this.RuleFor(x => x.dto.Observacion)
                     .MinimumLength(20).WithMessage("Debe tener al menos 20 caracteres")
                      .MaximumLength(250).WithMessage("Debe tener máximo 250 caracteres")
                      .Matches(@"^[a-zA-ZÀ-ÿ0-9\s\-_()]+$").WithMessage("No se permiten caracteres especiales");
            });
        }
    }
}
