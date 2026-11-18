namespace UsuariosAPI.Application.Features.Usuario.CommandAsegurarUsuarioInterno
{
    using UsuariosAPI.Application.Abstractions.Messaging;
    using UsuariosAPI.SDK.Contracts.DTOs.Usuario;

    public sealed record AsegurarUsuarioInternoCommand(AsegurarUsuarioInternoDTO dto) : ICommand<UsuarioDTO>;
}