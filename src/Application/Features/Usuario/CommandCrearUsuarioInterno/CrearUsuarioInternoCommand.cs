namespace UsuariosAPI.Application.Features.Usuario.CommandCrearUsuarioInterno
{
    using UsuariosAPI.Application.Abstractions.Messaging;
    using UsuariosAPI.SDK.Contracts.DTOs.Usuario;

    public sealed record CrearUsuarioInternoCommand(CrearUsuarioInternoDTO dto) : ICommand;
}
