namespace UsuariosAPI.Application.Features.Usuario.QueryUsuariosIds
{
    using UsuariosAPI.Application.Abstractions.Messaging;
    using UsuariosAPI.SDK.Contracts.DTOs.Usuario;

    public sealed record UsuariosIdsQuery(GetUsuariosIdsDTO dto) : IQuery<List<int>>;
}
