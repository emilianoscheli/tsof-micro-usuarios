namespace UsuariosAPI.Application.Features.Usuario.CommandActivarUsuarioExterno
{
    using UsuariosAPI.Application.Abstractions.Messaging;
    using UsuariosAPI.SDK.Contracts.DTOs.Usuario;

    public sealed record ActivarUsuarioExternoCommand(ActivarUsuarioExternoDTO dto) : ICommand;
}
