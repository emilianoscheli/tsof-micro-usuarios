namespace UsuariosAPI.Application.Features.Usuario.QueryUsuarioByIdRed
{
    using UsuariosAPI.Application.Abstractions.Messaging;
    using UsuariosAPI.SDK.Contracts.DTOs.Usuario;

    public sealed record UsuarioByIdRedQuery(GetUsuarioByIdRedDTO dto) : IQuery<UsuarioDTO>
    {
    }
}
