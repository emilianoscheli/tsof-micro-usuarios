namespace UsuariosAPI.Application.Features.DelegacionTemporal.CommandCreateDelegacionTemporal
{
    using UsuariosAPI.Application.Abstractions.Messaging;
    using UsuariosAPI.SDK.Contracts.DTOs.DelegacionTemporal;

    public record CreateDelegacionTemporalCommand(CreateDelegacionTemporalDTO dto) : ICommand;
}
