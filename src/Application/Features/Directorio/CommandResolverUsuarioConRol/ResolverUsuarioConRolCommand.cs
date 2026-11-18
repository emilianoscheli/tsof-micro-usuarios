namespace UsuariosAPI.Application.Features.Directorio.CommandResolverUsuarioConRol
{
    using UsuariosAPI.Application.Abstractions.Messaging;
    using UsuariosAPI.SDK.Contracts.DTOs.Directorio;

    public sealed record ResolverUsuarioConRolCommand(ResolverUsuarioConRolDTO dto)
        : ICommand<UsuarioDirectorioResueltoDTO>;
}
