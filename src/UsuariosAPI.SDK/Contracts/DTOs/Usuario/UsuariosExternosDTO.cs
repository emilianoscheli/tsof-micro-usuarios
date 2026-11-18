namespace UsuariosAPI.SDK.Contracts.DTOs.Usuario
{
    public class UsuariosExternosDTO
    {
        public UsuariosExternosDTO()
        {
        }

        public int Codigo { get; set; } = 0;

        public string NombreCompleto { get; set; } = string.Empty;

        public string IdRed { get; set; } = string.Empty;

        public string CorreoElectronico { get; set; } = string.Empty;

        public string Rol { get; set; } = string.Empty;

        public string Contratista { get; set; } = string.Empty;

        public string SedeEstudioAuditor { get; set; } = string.Empty;

        public bool Activo { get; set; } = true;

        public string Estado => this.Activo ? "Activo" : "Inactivo";
    }
}
