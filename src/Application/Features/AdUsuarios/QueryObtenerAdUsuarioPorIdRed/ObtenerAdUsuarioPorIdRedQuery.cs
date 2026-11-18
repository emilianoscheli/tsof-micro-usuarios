namespace UsuariosAPI.Application.Features.AdUsuarios.QueryObtenerAdUsuarioPorIdRed
{
    using UsuariosAPI.Application.Abstractions.Messaging;
    using UsuariosAPI.SDK.Contracts.DTOs.AdUsuario;

    public record ObtenerAdUsuarioPorIdRedQuery(string IdRed) : IQuery<AdUsuarioDto>
    {
    }
}
