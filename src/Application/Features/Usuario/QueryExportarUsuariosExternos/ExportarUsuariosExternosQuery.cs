namespace UsuariosAPI.Application.Features.Usuario.QueryExportarUsuariosExternos
{
    using APICore.Responses.DTOs;
    using UsuariosAPI.Application.Abstractions.Messaging;
    using UsuariosAPI.SDK.Contracts.DTOs.Usuario;

    public sealed record ExportarUsuariosExternosQuery(GetUsuariosExternosDTO dto, bool gdiAuth = true) : IQuery<FileResponseDTO>;
}
