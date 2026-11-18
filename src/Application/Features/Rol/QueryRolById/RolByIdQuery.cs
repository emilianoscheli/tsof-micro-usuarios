namespace UsuariosAPI.Application.Features.Rol.QueryRolById
{
    using APICore.Requests.DTOs;
    using APICore.Responses.DTOs;
    using UsuariosAPI.Application.Abstractions.Messaging;

    public sealed record RolByIdQuery(GetByIdDTO dto) : IQuery<RolDTO>
    {
    }
}
