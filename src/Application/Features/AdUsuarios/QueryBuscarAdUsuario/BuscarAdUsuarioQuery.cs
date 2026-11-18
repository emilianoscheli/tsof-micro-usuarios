namespace UsuariosAPI.Application.Features.AdUsuarios.QueryBuscarAdUsuario
{
    using UsuariosAPI.Application.Abstractions.Messaging;
    using UsuariosAPI.SDK.Contracts.DTOs.AdUsuario;

    public record BuscarAdUsuarioQuery(string palabraClave) : IQuery<List<AdUsuarioDto>>
    {
    }
}
