namespace UsuariosAPI.Application.Features.Usuario.CommandModificarRolesSociedades
{
    using UsuariosAPI.Application.Abstractions.Messaging;
    using UsuariosAPI.SDK.Contracts.DTOs.Usuario;

    public record ModificarRolesSociedadesCommand(UpdateUsuarioRolesSociedadesDTO dto ) : ICommand
    {
    }
}
