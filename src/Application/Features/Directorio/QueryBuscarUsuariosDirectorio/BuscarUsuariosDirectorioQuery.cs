namespace UsuariosAPI.Application.Features.Directorio.QueryBuscarUsuariosDirectorio
{
    using UsuariosAPI.Application.Abstractions.Messaging;
    using UsuariosAPI.SDK.Contracts.DTOs.Directorio;

    public sealed record BuscarUsuariosDirectorioQuery(BuscarUsuarioDirectorioDTO dto)
        : IQuery<List<UsuarioDirectorioDTO>>;
}
