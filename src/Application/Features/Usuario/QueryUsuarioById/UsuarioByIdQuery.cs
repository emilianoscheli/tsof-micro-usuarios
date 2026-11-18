namespace UsuariosAPI.Application.Features.Usuario.QueryUsuarioById
{
    using APICore.Requests.DTOs;
    using UsuariosAPI.Application.Abstractions.Messaging;
    using UsuariosAPI.SDK.Contracts.DTOs.Usuario;

    public sealed record UsuarioByIdQuery(GetByIdDTO dto, bool gdiAuth) : IQuery<UsuarioDTO>
    {
    }
}
