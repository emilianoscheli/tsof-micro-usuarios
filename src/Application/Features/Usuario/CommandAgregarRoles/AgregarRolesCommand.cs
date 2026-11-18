namespace UsuariosAPI.Application.Features.Usuario.CommandAgregarRoles
{
    using UsuariosAPI.Application.Abstractions.Messaging;
    using UsuariosAPI.SDK.Contracts.DTOs.Usuario;

    public record AgregarRolesCommand(AgregarRolesUsuarioDTO dto ) : ICommand
    {
    }
}
