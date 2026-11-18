namespace UsuariosAPI.Application.Features.AdUsuarios.QueryBuscarAdUsuarioExistente
{
    using UsuariosAPI.Application.Abstractions.Messaging;
    using UsuariosAPI.SDK.Contracts.DTOs.Usuario;

    public record BuscarAdUsuarioExistenteQuery(string? palabraClave, int? rolId = null) : IQueryPaginated<UsuariosDTO>;
}