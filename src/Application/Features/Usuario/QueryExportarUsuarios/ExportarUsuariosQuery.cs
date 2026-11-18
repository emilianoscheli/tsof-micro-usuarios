namespace UsuariosAPI.Application.Features.Usuario.QueryExportarUsuarios
{
    using APICore.Responses.DTOs;
    using UsuariosAPI.Application.Abstractions.Messaging;
    using UsuariosAPI.SDK.Contracts.DTOs.Usuario;

    public sealed record ExportarUsuariosQuery(GetUsuariosDTO dto) : IQuery<FileResponseDTO>
    {
    }
}
