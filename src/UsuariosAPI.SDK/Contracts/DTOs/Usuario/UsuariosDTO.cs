namespace UsuariosAPI.SDK.Contracts.DTOs.Usuario
{
    public class UsuariosDTO
    {
        public UsuariosDTO()
        {
        }

        public int Codigo { get; set; } = 0;

        public string NombreCompleto { get; set; } = string.Empty;

        public string IdRed { get; set; } = string.Empty;

        public string CorreoElectronico { get; set; } = string.Empty;

        public string Roles { get; set; } = string.Empty;

        public string Sociedades { get; set; } = string.Empty;

        public string Contratista { get; set; } = string.Empty;

        public string SedeEstudioAuditor { get; set; } = string.Empty;

        public string CodigoUO { get; set; } = string.Empty;
    }
}
