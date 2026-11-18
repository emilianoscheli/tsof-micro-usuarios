namespace UsuariosAPI.SDK.Contracts.DTOs.Usuario
{
    using APICore.Responses.DTOs;

    public class RolesTemporalesDTO
    {
        public List<RolDTO>? Rol { get; set; }

        public string? DelegadorRedId { get; set; }

        public string? NombreCompletoDelegador { get; set; }

        public DateOnly? FechaDesde { get; set; }

        public DateOnly? FechaHasta { get; set; }
    }
}
