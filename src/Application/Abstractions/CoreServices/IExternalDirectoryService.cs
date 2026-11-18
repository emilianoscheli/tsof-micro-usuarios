namespace UsuariosAPI.Application.Abstractions.CoreServices
{
    using APICore.Responses;
    using UsuariosAPI.SDK.Contracts.DTOs.AdUsuario;

    public interface IExternalDirectoryService
    {
        Task<(List<AdUsuarioDto> Data, Error? Error)> Search(string keyword);
    }
}
