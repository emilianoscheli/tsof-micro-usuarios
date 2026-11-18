namespace UsuariosAPI.Application.Features.Usuario.CommandCambiarRolUsuarioExterno
{
    using UsuariosAPI.Application.Abstractions.Messaging;
    using UsuariosAPI.SDK.Contracts.DTOs.Usuario;

    public sealed record CambiarRolUsuarioExternoCommand(CambiarRolUsuarioExternoDTO dto) : ICommand;
}
