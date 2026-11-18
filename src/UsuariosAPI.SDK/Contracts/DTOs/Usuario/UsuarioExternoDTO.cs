namespace UsuariosAPI.SDK.Contracts.DTOs.Usuario
{
    using UsuariosAPI.SDK.Contracts.DTOs.Contratista;
    using UsuariosAPI.SDK.Contracts.DTOs.EstudioAuditor;
    using UsuariosAPI.SDK.Contracts.DTOs.Sede;

    public class UsuarioExternoDTO
    {
        public UsuarioExternoDTO()
        {
        }

        public int Codigo { get; set; } = 0;

        public string NombreCompleto { get; set; } = string.Empty;

        public string IdRed { get; set; } = string.Empty;

        public string CorreoElectronico { get; set; } = string.Empty;

        public string Rol { get; set; } = string.Empty;

        public int RolId { get; set; } = 0;

        public string? Contratista { get; set; }

        public string? EstudioAuditor { get; set; }

        public string? Sede { get; set; }

        public string TipoDocumento { get; set; } = string.Empty;

        public string NumeroDocumento { get; set; } = string.Empty;

        public string CUIL { get; set; } = string.Empty;

        public string Pais { get; set; } = string.Empty;

        public string EmailEmpresa { get; set; } = string.Empty;

        public string TelefonoParticular { get; set; } = string.Empty;

        public string CodigoIncidencia { get; set; } = string.Empty;

        public bool Activo { get; set; } = true;

        public string Estado => this.Activo ? "Activo" : "Inactivo";
    }
}
