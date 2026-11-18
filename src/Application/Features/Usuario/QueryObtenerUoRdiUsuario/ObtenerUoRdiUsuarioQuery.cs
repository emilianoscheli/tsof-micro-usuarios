namespace UsuariosAPI.Application.Features.Usuario.QueryObtenerUoRdiUsuario
{
    using APICore.Requests.DTOs;
    using UsuariosAPI.Application.Abstractions.Messaging;
    using UsuariosAPI.SDK.Contracts.DTOs.Usuario;

    public sealed record ObtenerUoRdiUsuarioQuery(GetByIdDTO dto) : IQuery<UsuarioSolicitanteDTO>
    {
    }
}
