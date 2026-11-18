namespace UsuariosAPI.Application.Features.Usuario.QueryObtenerUoRdiPorUsuarios
{
    using UsuariosAPI.Application.Abstractions.Messaging;
    using UsuariosAPI.SDK.Contracts.DTOs.Usuario;

    public sealed record ObtenerUoRdiPorUsuariosQuery(GetUoRdiPorUsuariosDTO dto) : IQuery<List<UsuarioSolicitanteDTO>>
    {
    }
}
