namespace UsuariosAPI.Application.Features.Usuario.QueryUsuarioExternoById
{
    using APICore.Requests.DTOs;
    using UsuariosAPI.Application.Abstractions.Messaging;
    using UsuariosAPI.SDK.Contracts.DTOs.Usuario;

    public sealed record UsuarioExternoByIdQuery(GetByIdDTO dto, bool gdiAuth = true) : IQuery<UsuarioExternoDTO>;
}
