namespace UsuariosAPI.Application.Features.Usuario.QueryRolesCompatibles
{
    using APICore.Requests.DTOs;
    using APICore.Responses.DTOs;
    using UsuariosAPI.Application.Abstractions.Messaging;

    public record RolesCompatiblesQuery(GetByIdDTO dto) : IQuery<List<RolDTO>>;
}

