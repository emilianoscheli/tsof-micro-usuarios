namespace UsuariosAPI.Application.Features.Usuario.CommandDesactivarUsuarioExterno
{
    using UsuariosAPI.Application.Abstractions.Messaging;
    using UsuariosAPI.SDK.Contracts.DTOs.Usuario;

    public sealed record DesactivarUsuarioExternoCommand(DesactivarUsuarioExternoDTO dto) : ICommand;
}
