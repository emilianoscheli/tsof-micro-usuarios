namespace UsuariosAPI.Application.Features.Usuario.CommandCrearUsuarioExterno
{
    using UsuariosAPI.Application.Abstractions.Messaging;
    using UsuariosAPI.SDK.Contracts.DTOs.Usuario;

    public sealed record CrearUsuarioExternoCommand(CrearUsuarioExternoDTO dto) : ICommand;
}
