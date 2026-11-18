namespace UsuariosAPI.Application.Features.Usuario.QueryUsuariosExternosIds
{
    using UsuariosAPI.Application.Abstractions.Messaging;
    using UsuariosAPI.SDK.Contracts.DTOs.Usuario;

    public sealed record UsuariosExternosIdsQuery(GetUsuariosExternosIdsDTO dto) : IQuery<List<int>>;
}
