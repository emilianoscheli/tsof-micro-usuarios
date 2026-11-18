namespace UsuariosAPI.Application.Features.Usuario.CommandEliminarRolesUsuarioBatch
{
    using UsuariosAPI.Application.Abstractions.Messaging;
    using UsuariosAPI.SDK.Contracts.DTOs.Usuario;

    public record EliminarRolesUsuarioBatchCommand(UpdateUsuarioRolesBatchDTO dto) : ICommand
    {
    }
}
