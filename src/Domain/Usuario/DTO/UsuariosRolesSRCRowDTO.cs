namespace UsuariosAPI.Domain.Usuario.DTO
{
    public class UsuariosRolesSRCRowDTO
    {
        public UsuariosRolesSRCRowDTO()
        {
        }

        public string IdUsuario { get; set; } = string.Empty;

        public string ApellidoNombreUsuario { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public int RolId { get; set; }

        public string RolEnSRC { get; set; } = string.Empty;

        public string TipoAsignacionDelRol { get; set; } = string.Empty;

        public List<int> SociedadesIds { get; set; } = new ();

        public int? ContratistaFkId { get; set; }

        public int? EstudioAuditorFkId { get; set; }
    }
}
