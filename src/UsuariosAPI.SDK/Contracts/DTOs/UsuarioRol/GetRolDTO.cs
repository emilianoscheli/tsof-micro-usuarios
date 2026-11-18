namespace UsuariosAPI.SDK.Contracts.DTOs.UsuarioRol
{
    using APICore.Requests;

    public class GetRolDTO : PaginatedOrderedRequest
    {
        public GetRolDTO()
        {
        }

        public string? Nombre { get; init; } = null;

        public bool? AsignaSociedad { get; init; } = null;

        public bool? EsManual { get; init; } = null;
    }
}
