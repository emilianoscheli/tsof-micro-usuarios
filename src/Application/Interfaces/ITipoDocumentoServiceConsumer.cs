namespace UsuariosAPI.Application.Interfaces
{
    using APICore.Responses;
    using APICore.Responses.DTOs;

    public interface ITipoDocumentoServiceConsumer
    {
        Task<(EnumDTO?, Error?)> GetTipoDocumentoById(int id, bool gdiAuth = true);

        Task<(List<EnumDTO?>?, Error?)> GetAllTiposDocumentosAsync(bool gdiAuth = true);
    }
}
