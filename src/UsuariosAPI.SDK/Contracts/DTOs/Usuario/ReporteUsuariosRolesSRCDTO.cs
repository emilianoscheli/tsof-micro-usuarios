namespace UsuariosAPI.SDK.Contracts.DTOs.Usuario
{
    public class ReporteUsuariosRolesSRCDTO
    {
        public ReporteUsuariosRolesSRCDTO()
        {
        }

        public string IdUsuario { get; set; } = string.Empty;

        public string ApellidoNombreUsuario { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string TipoAsignacionDelRol { get; set; } = string.Empty;

        public string RolEnSRC { get; set; } = string.Empty;

        public string Sociedades { get; set; } = string.Empty;

        public int? ContratistaId { get; set; }

        public string Contratista { get; set; } = string.Empty;

        public string ContratistaCuit { get; set; } = string.Empty;

        public string EstudioAuditor { get; set; } = string.Empty;

        public bool? Activo { get; set; }
    }
}
