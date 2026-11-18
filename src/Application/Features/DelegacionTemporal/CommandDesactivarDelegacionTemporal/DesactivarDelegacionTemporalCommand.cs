namespace UsuariosAPI.Application.Features.DelegacionTemporal.CommandDesactivarDelegacionTemporal
{
    using APICore.Requests.DTOs;
    using UsuariosAPI.Application.Abstractions.Messaging;

    public record DesactivarDelegacionTemporalCommand(CambiarEstadoConMotivoDTO dto) : ICommand;
}
