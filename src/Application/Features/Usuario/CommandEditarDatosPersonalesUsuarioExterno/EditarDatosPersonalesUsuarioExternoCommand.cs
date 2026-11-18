namespace UsuariosAPI.Application.Features.Usuario.CommandEditarDatosPersonalesUsuarioExterno
{
    using UsuariosAPI.Application.Abstractions.Messaging;
    using UsuariosAPI.SDK.Contracts.DTOs.Usuario;

    public sealed record EditarDatosPersonalesUsuarioExternoCommand(EditarDatosPersonalesUsuarioExternoDTO dto) : ICommand;
}
