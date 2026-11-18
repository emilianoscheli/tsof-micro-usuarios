namespace UsuariosAPI.SDK.Contracts.DTOs.Usuario
{
    public class UsuarioAcotadoDTO
    {
        public UsuarioAcotadoDTO()
        {
        }

        public int Id { get; set; } = 0;

        public string NombreCompleto { get; set; } = string.Empty;

        public string IdRed { get; set; } = string.Empty;

        public string CorreoElectronico { get; set; } = string.Empty;

        public string CodigoUO { get; set; } = string.Empty;

        public List<int> SociedadesIds { get; set; } = new List<int>();

        public List<int> RolesIds { get; set; } = new List<int>();

        public List<int> RolesTemporalesIds { get; set; } = new List<int>();

        public int? ContratistaId { get; set; }

        public int? EstudioAuditorId { get; set; }

        public int? SedeId { get; set; }

        public string? Legajo { get; set; }
    }
}
