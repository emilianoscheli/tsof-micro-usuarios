namespace UsuariosAPI.SDK.Contracts.DTOs.Usuario
{
    using UsuariosAPI.SDK.Contracts.DTOs.Contratista;
    using UsuariosAPI.SDK.Contracts.DTOs.EstudioAuditor;
    using UsuariosAPI.SDK.Contracts.DTOs.Sede;
    using UsuariosAPI.SDK.Contracts.DTOs.UsuarioRol;
    using UsuariosAPI.SDK.Contracts.DTOs.UsuarioSociedad;

    public class UsuarioDTO
    {
        public UsuarioDTO()
        {
        }

        public int Codigo { get; set; } = 0;

        public string NombreCompleto { get; set; } = string.Empty;

        public string IdRed { get; set; } = string.Empty;

        public string CorreoElectronico { get; set; } = string.Empty;

        public string Roles { get; set; } = string.Empty;

        public List<UsuariosRolesDTO> RolesList { get; set; } = new List<UsuariosRolesDTO>();

        public string Sociedades { get; set; } = string.Empty;

        public List<UsuarioSociedadDTO> SociedadesList { get; set; } = new List<UsuarioSociedadDTO>();

        public ContratistaUsuarioDTO? Contratista { get; set; }

        public EstudioAuditorUsuarioDTO? EstudioAuditor { get; set; }

        public SedeUsuarioDTO? Sede { get; set; }

        public string CodigoUO { get; set; } = string.Empty;

        public string? Legajo { get; set; }

        public List<RolesTemporalesDTO>? RolesTemporales { get; set; }
    }
}
